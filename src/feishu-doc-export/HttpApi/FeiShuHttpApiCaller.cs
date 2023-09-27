using feishu_doc_export.Dtos;
using feishu_doc_export.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace feishu_doc_export.HttpApi
{
    public interface IFeiShuHttpApiCaller
    {
        #region 知识空间Wiki
        /// <summary>
        /// 获取空间子节点列表
        /// </summary>
        /// <param name="spaceId">知识空间Id</param>
        /// <param name="pageToken">分页token，第一次查询没有</param>
        /// <param name="parentNodeToken">父节点token</param>
        /// <returns></returns>
        Task<PagedResult<WikiNodeItemDto>> GetWikiNodeList(string spaceId, string pageToken = null, string parentNodeToken = null);

        /// <summary>
        /// 获取知识空间下全部文档节点
        /// </summary>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        Task<List<WikiNodeItemDto>> GetAllWikiNode(string spaceId);

        /// <summary>
        /// 递归获取知识空间下指定节点下的所有子节点（包括孙节点）
        /// </summary>
        /// <param name="spaceId">知识空间id</param>
        /// <param name="parentNodeToken">父节点token</param>
        /// <returns></returns>
        Task<List<WikiNodeItemDto>> GetWikiChildNode(string spaceId, string parentNodeToken);

        /// <summary>
        /// 获取所有的知识库
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<WikiSpaceDto>> GetWikiSpaces();

        /// <summary>
        /// 获取知识库详细信息
        /// </summary>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        Task<WikiSpaceInfo> GetWikiSpaceInfo(string spaceId);
        #endregion

        #region 下载文档
        /// <summary>
        /// 创建导出任务
        /// </summary>
        /// <param name="fileExtension">导出文件扩展名</param>
        /// <param name="token">文档token</param>
        /// <param name="type">导出文档类型</param>
        /// <returns></returns>
        Task<ExportOutputDto> CreateExportTask(string fileExtension, string token, string type);

        /// <summary>
        /// 查询导出任务的结果
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task<ExportTaskResultDto> QueryExportTaskResult(string ticket, string token);

        /// <summary>
        /// 下载文档文件
        /// </summary>
        /// <param name="fileToken"></param>
        /// <returns></returns>
        Task<byte[]> DownLoad(string fileToken);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileToken"></param>
        /// <returns></returns>
        Task<byte[]> DownLoadFile(string fileToken);
        #endregion

        #region 个人空间云文档
        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        /// <param name="folderToken"></param>
        /// <returns></returns>
        Task<CloudDocFolderMeta> GetFolderMeta(string folderToken);
        /// <summary>
        /// 获取个人空间云文档
        /// </summary>
        /// <param name="folderToken"></param>
        /// <param name="pageToken"></param>
        /// <returns></returns>
        Task<PagedResult<CloudDocDto>> GetCloudDocList(string folderToken = null, string pageToken = null);

        /// <summary>
        /// 获取个人空间云文档指定文件夹下的所有文档
        /// </summary>
        /// <param name="folderToken"></param>
        /// <returns></returns>
        Task<List<CloudDocDto>> GetFolderAllCloudDoc(string folderToken);

        /// <summary>
        /// 递归获取子文档
        /// </summary>
        /// <param name="parentNodeToken"></param>
        /// <returns></returns>
        Task<List<CloudDocDto>> GetChildCloudDoc(string parentNodeToken);
        #endregion
    }
    public class FeiShuHttpApiCaller : IFeiShuHttpApiCaller
    {
        private readonly IFeiShuHttpApi _feiShuHttpApi;

        public FeiShuHttpApiCaller(IFeiShuHttpApi feiShuHttpApi)
        {
            _feiShuHttpApi = feiShuHttpApi;
        }

        #region 获取知识库所有的文档节点

        public async Task<PagedResult<WikiNodeItemDto>> GetWikiNodeList(string spaceId, string pageToken = null, string parentNodeToken = null)
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

            var resultData = await _feiShuHttpApi.GetWikeNodeList(urlBuilder.ToString());

            return resultData.Data;
        }

        public async Task<List<WikiNodeItemDto>> GetAllWikiNode(string spaceId)
        {
            try
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
            catch (HttpRequestException ex)
            {
                LogHelper.LogError($"请求异常！！！请检查您的网络环境。异常信息：{ex.Message}");
                throw;
            }
        }

        public async Task<List<WikiNodeItemDto>> GetWikiChildNode(string spaceId, string parentNodeToken)
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
        public async Task<byte[]> DownLoadFile(string fileToken)
        {
            try
            {
                var result = await _feiShuHttpApi.DownLoadFile(fileToken);

                return result;
            }
            catch (HttpRequestException ex)
            {
                LogHelper.LogError($"请求异常！！！请检查您的网络环境。异常信息：{ex.Message}");
                throw;
            }
        }

        public async Task<ExportOutputDto> CreateExportTask(string fileExtension, string token, string type)
        {
            var request = RequestData.CreateExportTask(fileExtension, token, type);

            try
            {
                var result = await _feiShuHttpApi.CreateExportTask(request);
                return result.Data;
            }
            catch (HttpRequestException ex) when (ex.InnerException is ApiResponseStatusException statusException)
            {
                // 响应状态码异常
                var response = statusException.ResponseMessage;

                // 响应的数据
                var responseData = await response.Content.ReadAsStringAsync();

                if (responseData.Contains("1069902"))
                {
                    string message = $"无阅读或导出权限，已忽略。飞书服务端响应数据为：{responseData}";
                    throw new CustomException(message, 1069902);
                }
            }

            return null;
        }

        public async Task<ExportTaskResultDto> QueryExportTaskResult(string ticket, string token)
        {
            int status;// 0成功，1初始化，2处理中

            var data = new ExportTaskResultDto();
            do
            {
                var result = await _feiShuHttpApi.QueryExportTask(ticket, token);

                status = result.Data.Result.JobStatus;

                switch (status)
                {
                    case 0:
                        data = result.Data;
                        break;
                    case 1:
                    case 2:
                        await Task.Delay(300);
                        break;
                    default:
                        throw new Exception($"Error: {result.Data.Result.JobErrorMsg}，ErrorCode:{status}");
                }

            } while (status != 0);

            return data;
        }

        public async Task<byte[]> DownLoad(string fileToken)
        {
            var result = await _feiShuHttpApi.DownLoad(fileToken);

            return result;
        }

        #endregion

        #region 知识库
        public async Task<PagedResult<WikiSpaceDto>> GetWikiSpaces()
        {
            try
            {
                var res = await _feiShuHttpApi.GetWikiSpaces();

                return res.Data;
            }
            catch (HttpRequestException ex)
            {
                LogHelper.LogError($"请求异常！！！请检查您的网络环境。异常信息：{ex.Message}");
                throw;
            }
        }

        public async Task<WikiSpaceInfo> GetWikiSpaceInfo(string spaceId)
        {
            try
            {
                var res = await _feiShuHttpApi.GetWikiSpaceInfo(spaceId);

                return res.Data;
            }
            catch (HttpRequestException ex)
            {
                LogHelper.LogError($"请求异常！！！请检查您的网络环境。异常信息：{ex.Message}");
                throw;
            }
        }
        #endregion

        #region 个人空间云文档
        public async Task<CloudDocFolderMeta> GetFolderMeta(string folderToken)
        {
            try
            {
                var res = await _feiShuHttpApi.GetFolderMeta(folderToken);

                return res.Data;
            }
            catch (HttpRequestException ex)
            {
                LogHelper.LogError($"请求异常！！！请检查您的网络环境。异常信息：{ex.Message}");
                throw;
            }
        }

        public async Task<PagedResult<CloudDocDto>> GetCloudDocList(string folderToken = null, string pageToken = null)
        {
            StringBuilder urlBuilder = new StringBuilder($"{FeiShuConsts.OpenApiEndPoint}/open-apis/drive/v1/files?folder_token={folderToken}&page_size=50");// page_size=50
            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                urlBuilder.Append($"&page_token={pageToken}");
            }

            var resultData = await _feiShuHttpApi.GetCloudDocList(urlBuilder.ToString());

            return resultData.Data;
        }

        public async Task<List<CloudDocDto>> GetFolderAllCloudDoc(string folderToken)
        {
            try
            {
                List<CloudDocDto> nodes = new List<CloudDocDto>();
                string pageToken = null;
                bool hasMore;
                do
                {
                    // 分页获取顶级节点，pageToken = null时为获取第一页
                    var pagedResult = await GetCloudDocList(folderToken, pageToken);
                    nodes.AddRange(pagedResult.Files);

                    foreach (var item in pagedResult.Files)
                    {
                        if (item.Type == "folder")
                        {
                            List<CloudDocDto> childNodes = await GetChildCloudDoc(item.Token);
                            nodes.AddRange(childNodes);
                        }
                    }

                    pageToken = pagedResult.PageToken;
                    hasMore = pagedResult.HasMore;

                } while (hasMore && !string.IsNullOrWhiteSpace(pageToken));

                return nodes;

            }
            catch (HttpRequestException ex)
            {
                LogHelper.LogError($"请求异常！！！请检查您的网络环境。异常信息：{ex.Message}");
                throw;
            }
        }

        public async Task<List<CloudDocDto>> GetChildCloudDoc(string parentNodeToken)
        {
            List<CloudDocDto> childNodes = new List<CloudDocDto>();
            string pageToken = null;
            bool hasMore;
            do
            {
                var pagedResult = await GetCloudDocList(parentNodeToken, pageToken);
                childNodes.AddRange(pagedResult.Files);

                foreach (var item in pagedResult.Files)
                {
                    if (item.Type == "folder")
                    {
                        List<CloudDocDto> grandChildNodes = await GetChildCloudDoc(item.Token);
                        childNodes.AddRange(grandChildNodes);
                    }
                }

                pageToken = pagedResult.PageToken;
                hasMore = pagedResult.HasMore;

            } while (hasMore && !string.IsNullOrWhiteSpace(pageToken));

            return childNodes;
        }

        #endregion

    }
}
