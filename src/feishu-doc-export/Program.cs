
using feishu_doc_export.Dtos;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.RegularExpressions;
using WebApiClientCore;
using WebApiClientCore.Exceptions;

namespace feishu_doc_export
{
    internal class Program
    {
        static IFeiShuHttpApi feiShuHttpApi;

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                GlobalConfig.AppId = GetCommandLineArg(args, "--appId=");
                GlobalConfig.AppSecret = GetCommandLineArg(args, "--appSecret=");
                GlobalConfig.WikiSpaceId = GetCommandLineArg(args, "--spaceId=");
                GlobalConfig.ExportPath = GetCommandLineArg(args, "--exportPath=");
            }
            else
            {
                Console.WriteLine("请输入飞书自建应用的AppId");
                GlobalConfig.AppId = Console.ReadLine();
                Console.WriteLine("请输入飞书自建应用的AppSecret");
                GlobalConfig.AppSecret = Console.ReadLine();
                Console.WriteLine("请输入要导出的知识库Id");
                GlobalConfig.WikiSpaceId = Console.ReadLine();
                Console.WriteLine("请输入文档导出的目录位置");
                GlobalConfig.ExportPath = Console.ReadLine();
            }


            IOC.Init();

            feiShuHttpApi = IOC.IoContainer.GetService<IFeiShuHttpApi>();

            Console.WriteLine("正在加载知识库的所有文档信息，请耐心等待...");

            // 获取知识库下的所有文档
            var wikiNodes = await GetAllWikiNode(GlobalConfig.WikiSpaceId);

            // 不支持导出的文件
            List<string> noSupportExportFiles = new List<string>();

            // 记录导出的文档数量
            int count = 1;
            foreach (var item in wikiNodes)
            {

                var isSupport = GlobalConfig.GetFileExtension(item.ObjType, out string fileExt);
                // 如果该文件类型不支持导出
                if (!isSupport)
                {
                    noSupportExportFiles.Add(item.Title);
                    Console.WriteLine($"文档【{item.Title}】不支持导出，已忽略。如有需要请手动下载");
                    continue;
                }

                // 文件名超出长度限制，不支持导出
                if (item.Title.Length > 64)
                {
                    var left64FileName = item.Title.PadLeft(61) + $"···.{fileExt}";
                    noSupportExportFiles.Add($"(文件名超长){left64FileName}");
                    Console.WriteLine($"文档【{left64FileName}】的文件命名长度超出Windows文件命名的长度限制，已忽略");
                    continue;
                }

                Console.WriteLine($"正在导出文档————————{count++}.【{item.Title}.{fileExt}】");

                await DownLoadDocument(fileExt, item.ObjToken, item.ObjType);
            }
            Console.WriteLine("—————————————————————————————文档已全部导出—————————————————————————————");
            Console.WriteLine(noSupportExportFiles.Any() ? "以下是所有不支持导出的文档" : "");

            // 输出不支持导入的文档
            for (int i = 0; i < noSupportExportFiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}.【{noSupportExportFiles[i]}】");
            }
        }

        /// <summary>
        /// 获取命令行参数值
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        static string GetCommandLineArg(string[] args, string parameterName)
        {
            // 参数值
            string paraValue = string.Empty;
            // 是否有匹配的参数
            bool found = false;
            foreach (string arg in args)
            {
                if (arg.StartsWith(parameterName))
                {
                    paraValue = arg.Substring(parameterName.Length);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine($"没有找到参数：{parameterName}.");
                Console.WriteLine("请填写以下所有参数：");
                Console.WriteLine("  --appId           飞书自建应用的AppId.");
                Console.WriteLine("  --appSecret       飞书自建应用的AppSecret.");
                Console.WriteLine("  --spaceId         飞书导出的知识库Id.");
                Console.WriteLine("  --exportPath      文档导出的目录位置.");
                Environment.Exit(0);
            }

            // 参数值为空
            if (string.IsNullOrWhiteSpace(paraValue))
            {
                Console.WriteLine($"参数{parameterName}不能为空");
                Environment.Exit(0);
            }

            return paraValue;
        }

        #region 获取所有的文档节点

        /// <summary>
        /// 获取空间子节点列表
        /// </summary>
        /// <param name="spaceId">知识空间Id</param>
        /// <param name="pageToken">分页token，第一次查询没有</param>
        /// <param name="parentNodeToken">父节点token</param>
        /// <returns></returns>
        static async Task<WikiNodePagedResult> GetWikiNodeList(string spaceId, string pageToken = null, string parentNodeToken = null)
        {
            StringBuilder urlBuilder = new StringBuilder($"{FeiShuConsts.OpenApiEndPoint}/open-apis/wiki/v2/spaces/{spaceId}/nodes?page_size=50");// page_size=50
            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                urlBuilder.Append($"&page_token={pageToken}");
            }

            if (!string.IsNullOrWhiteSpace(parentNodeToken))
            {
                urlBuilder.Append($"&parent_node_token={parentNodeToken}");
            }

            var resultData = await feiShuHttpApi.GetWikeNodeList(urlBuilder.ToString());

            return resultData.Data;
        }

        /// <summary>
        /// 获取知识空间下全部文档节点
        /// </summary>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        static async Task<List<WikiNodeItemDto>> GetAllWikiNode(string spaceId)
        {
            List<WikiNodeItemDto> nodes = new List<WikiNodeItemDto>();
            string pageToken = null;
            bool hasMore;
            do
            {
                // 分页获取顶级节点，pageToken = null时为获取第一页
                var pagedResult = await GetWikiNodeList(spaceId, pageToken);
                nodes.AddRange(pagedResult.Items);

                foreach (var item in pagedResult.Items)
                {
                    if (item.HasChild)
                    {
                        List<WikiNodeItemDto> childNodes = await GetWikiChildNode(spaceId, item.NodeToken);
                        nodes.AddRange(childNodes);
                    }
                }

                pageToken = pagedResult.PageToken;
                hasMore = pagedResult.HasMore;

            } while (hasMore && !string.IsNullOrWhiteSpace(pageToken));

            return nodes;
        }

        /// <summary>
        /// 递归获取知识空间下指定节点下的所有子节点（包括孙节点）
        /// </summary>
        /// <param name="spaceId">知识空间id</param>
        /// <param name="parentNodeToken">父节点token</param>
        /// <returns></returns>
        static async Task<List<WikiNodeItemDto>> GetWikiChildNode(string spaceId, string parentNodeToken)
        {
            List<WikiNodeItemDto> childNodes = new List<WikiNodeItemDto>();
            string pageToken = null;
            bool hasMore;
            do
            {
                var pagedResult = await GetWikiNodeList(spaceId, pageToken, parentNodeToken);
                childNodes.AddRange(pagedResult.Items);

                foreach (var item in pagedResult.Items)
                {
                    if (item.HasChild)
                    {
                        List<WikiNodeItemDto> grandChildNodes = await GetWikiChildNode(spaceId, item.NodeToken);
                        childNodes.AddRange(grandChildNodes);
                    }
                }

                pageToken = pagedResult.PageToken;
                hasMore = pagedResult.HasMore;

            } while (hasMore && !string.IsNullOrWhiteSpace(pageToken));

            return childNodes;
        }
        #endregion

        #region 导出文档
        /// <summary>
        /// 创建导出任务
        /// </summary>
        /// <param name="fileExtension">导出文件扩展名</param>
        /// <param name="token">文档token</param>
        /// <param name="type">导出文档类型</param>
        /// <returns></returns>
        static async Task<ExportOutputDto> CreateExportTask(string fileExtension, string token, string type)
        {
            var request = RequestData.CreateExportTask(fileExtension, token, type);

            try
            {
                var result = await feiShuHttpApi.CreateExportTask(request);
                return result.Data;
            }
            catch (HttpRequestException ex) when (ex.InnerException is ApiResponseStatusException statusException)
            {
                // 响应状态码异常
                var response = statusException.ResponseMessage;

                // 响应的数据
                var responseData = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseData);
            }

            return null;
        }

        /// <summary>
        /// 查询导出任务的结果
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        static async Task<ExportTaskResultDto> QueryExportTaskResult(string ticket, string token)
        {
            int status;

            var data = new ExportTaskResultDto();
            do
            {
                var result = await feiShuHttpApi.QueryExportTask(ticket, token);

                status = result.Data.Result.JobStatus;

                switch (status)
                {
                    case 0:
                        data = result.Data;
                        break;
                    case 2:
                        await Task.Delay(300);
                        break;
                    default:
                        throw new Exception($"Error: {result.Data.Result.JobErrorMsg}，ErrorCode:{status}");
                }

            } while (status != 0);

            return data;
        }

        /// <summary>
        /// 下载文档文件
        /// </summary>
        /// <param name="fileToken"></param>
        /// <returns></returns>
        static async Task<byte[]> DownLoad(string fileToken)
        {
            var result = await feiShuHttpApi.DownLoad(fileToken);

            return result;
        }

        /// <summary>
        /// 下载文档到本地
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static async Task DownLoadDocument(string fileExtension, string token, string type)
        {
            var exportTaskDto = await CreateExportTask(fileExtension, token, type);

            var exportTaskResult = await QueryExportTaskResult(exportTaskDto.Ticket, token);
            var taskInfo = exportTaskResult.Result;

            if (taskInfo.JobErrorMsg == "success")
            {
                var bytes = await DownLoad(taskInfo.FileToken);

                var savefileName = taskInfo.FileName + "." + fileExtension;
                // 替换文件名中的非法字符
                savefileName = Regex.Replace(savefileName, @"[\\/:\*\?""<>\|]", "-");

                var filePath = Path.Combine(GlobalConfig.ExportPath, savefileName);

                File.WriteAllBytes(filePath, bytes);
            }
        }
        #endregion
    }
}