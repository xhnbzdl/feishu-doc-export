
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using feishu_doc_export.Helper;
using feishu_doc_export.HttpApi;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using WebApiClientCore;

namespace feishu_doc_export
{
    internal class Program
    {
        static IFeiShuHttpApiCaller feiShuApiCaller;

        static async Task Main(string[] args)
        {
            GlobalConfig.Init(args);

            if (!Directory.Exists(GlobalConfig.ExportPath))
            {
                LogHelper.LogError($"指定的导出目录({GlobalConfig.ExportPath})不存在！！！");
            }

            IOC.Init();
            feiShuApiCaller = IOC.IoContainer.GetService<IFeiShuHttpApiCaller>();

            if (string.IsNullOrWhiteSpace(GlobalConfig.WikiSpaceId))
            {
                var wikiSpaces = await feiShuApiCaller.GetWikiSpaces();
                var wikiSpaceDict = wikiSpaces.Items
                    .Select((x, i) => new { Index = i + 1, WikiSpace = x })
                    .ToDictionary(x => x.Index, x => x.WikiSpace);

                if (wikiSpaceDict.Any())
                {
                    Console.WriteLine($"以下是所有支持导出的知识库：");

                    foreach (var item in wikiSpaceDict)
                    {
                        Console.WriteLine($"【{item.Key}.】{item.Value.Name}");
                    }
                    Console.WriteLine("请选择知识库（输入知识库的序号）：");
                    var index = int.Parse(Console.ReadLine());
                    GlobalConfig.WikiSpaceId = wikiSpaceDict[index].Spaceid;
                }
                else
                {
                    LogHelper.LogError("没有可支持导出的知识库！！！");
                }
            }

            var wikiSpaceInfo = await feiShuApiCaller.GetWikiSpaceInfo(GlobalConfig.WikiSpaceId);

            Console.WriteLine($"正在加载知识库【{wikiSpaceInfo.Space.Name}】的所有文档信息，请耐心等待...");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 获取知识库下的所有文档
            var wikiNodes = await feiShuApiCaller.GetAllWikiNode(GlobalConfig.WikiSpaceId);

            // 文档路径映射字典
            DocumentPathGenerator.GenerateDocumentPaths(wikiNodes, GlobalConfig.ExportPath);

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
                    LogHelper.LogWarn($"文档【{item.Title}】不支持导出，已忽略。如有需要请手动下载");
                    continue;
                }

                // 用于展示的文件后缀名称
                var showFileExt = fileExt;

                if (fileExt == "docx")
                {
                    showFileExt = GlobalConfig.DocSaveType;
                }

                // 文件名超出长度限制，不支持导出
                if (item.Title.Length > 64)
                {
                    var left64FileName = item.Title.PadLeft(61) + $"···.{fileExt}";
                    noSupportExportFiles.Add($"(文件名超长){left64FileName}");
                    Console.WriteLine($"文档【{left64FileName}】的文件命名长度超出Windows文件命名的长度限制，已忽略");
                    continue;
                }

                Console.WriteLine($"正在导出文档————————{count++}.【{item.Title}.{showFileExt}】");

                await DownLoadDocument(fileExt, item.ObjToken, item.ObjType);
            }

            Console.WriteLine("—————————————————————————————文档已全部导出—————————————————————————————");
            Console.WriteLine(noSupportExportFiles.Any() ? "以下是所有不支持导出的文档" : "");

            // 输出不支持导入的文档
            for (int i = 0; i < noSupportExportFiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}.【{noSupportExportFiles[i]}】");
            }

            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            // 输出执行时间（以秒为单位）
            double seconds = elapsedTime.TotalSeconds;
            Console.WriteLine($"程序执行结束，总耗时{seconds}（秒）。请按任意键退出！");

            Console.ReadKey();
        }

        /// <summary>
        /// 下载文档到本地
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static async Task DownLoadDocument(string fileExtension, string objToken, string type)
        {
            var exportTaskDto = await feiShuApiCaller.CreateExportTask(fileExtension, objToken, type);

            var exportTaskResult = await feiShuApiCaller.QueryExportTaskResult(exportTaskDto.Ticket, objToken);
            var taskInfo = exportTaskResult.Result;

            if (taskInfo.JobErrorMsg == "success")
            {
                var bytes = await feiShuApiCaller.DownLoad(taskInfo.FileToken);

                var saveFileName = DocumentPathGenerator.GetDocumentPath(objToken) + "." + fileExtension;

                var filePath = Path.Combine(GlobalConfig.ExportPath, saveFileName);

                if (fileExtension == "docx" && GlobalConfig.DocSaveType == "md")
                {
                    await SaveToMarkdownFile(bytes, filePath);
                    return;
                }

                await filePath.Save(bytes);
            }
        }

        /// <summary>
        /// 保存为Markdown文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileSavePath"></param>
        static async Task SaveToMarkdownFile(byte[] bytes,string fileSavePath)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                // 加载 Word 文档
                Document doc = new Document(stream);

                // 遍历文档中的所有形状（包括图片）
                foreach (Shape shape in doc.GetChildNodes(NodeType.Shape, true))
                {
                    if (shape.HasImage)
                    {
                        // 清空图片描述
                        shape.AlternativeText = "";
                    }
                }

                // 创建Markdown保存选项
                MarkdownSaveOptions saveOptions = new MarkdownSaveOptions();
                // 文件保存的文件夹路径
                var saveDirPath = Path.GetDirectoryName(fileSavePath);
                // 设置文章中图片的存储路径
                saveOptions.ImagesFolder = Path.Combine(saveDirPath, "images");
                // 重构文件名
                var fileName = Path.GetFileNameWithoutExtension(fileSavePath) + ".md";
                // 文件最终的保存路径
                var mdFileSavePath = Path.Combine(saveDirPath, fileName);
                doc.Save(mdFileSavePath, saveOptions);

                // 处理 Markdown 文件，替换图片和文档的引用路径为相对路径
                var markdownContent = await File.ReadAllTextAsync(mdFileSavePath);
                var replacedContent = markdownContent.ReplaceImagePath(mdFileSavePath).ReplaceDocRefPath(mdFileSavePath).ReplaceCodeToMdFormat();
                await File.WriteAllTextAsync(mdFileSavePath, replacedContent);
            }

        }
        
    }
}