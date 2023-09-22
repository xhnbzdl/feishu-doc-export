using Aspose.Words;
using System.Security.Cryptography;
using System.Text;

namespace feishu_doc_export
{
    public static class GlobalConfig
    {
        public static string AppId { get; set; }

        public static string AppSecret { get; set; }

        public static string ExportPath { get; set; }

        public static string WikiSpaceId { get; set; }

        private static string _docSaveType = "docx";

        public static string DocSaveType { 
            get { return _docSaveType; }
            set
            {
                var options = new string[] { "pdf", "docx", "md" };

                _docSaveType = options.Contains(value) ? value : "docx";
            } 
        }

        /// <summary>
        /// 飞书支持导出的文件类型和导出格式
        /// </summary>
        static Dictionary<string, string> fileExtensionDict = new Dictionary<string, string>()
        {
            {"doc","docx" },
            {"docx","docx" },
            {"sheet","xlsx" },
            {"bitable","xlsx" },
            {"file","file" },
        };

        /// <summary>
        /// 获取飞书支持导出的文件格式
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public static bool GetFileExtension(string objType, out string fileExt)
        {
            return fileExtensionDict.TryGetValue(objType, out fileExt);
        }

        private static void InitAsposeLicense()
        {
            License license = new License();
            // 加载本地密钥
            license.SetLicense("C:\\Users\\User\\Desktop\\Aspose.lic");
        }

        /// <summary>
        /// 初始化全局配置信息
        /// </summary>
        /// <param name="args"></param>
        public static void Init(string[] args)
        {
            if (args.Length > 0)
            {
                AppId = GetCommandLineArg(args, "--appId=");
                AppSecret = GetCommandLineArg(args, "--appSecret=");
                WikiSpaceId = GetCommandLineArg(args, "--spaceId=", true);
                DocSaveType = GetCommandLineArg(args, "--saveType=", true);
                ExportPath = GetCommandLineArg(args, "--exportPath=");
            }
            else
            {
//#if !DEBUG
                Console.WriteLine("请输入飞书自建应用的AppId：");
                AppId = Console.ReadLine();
                Console.WriteLine("请输入飞书自建应用的AppSecret：");
                AppSecret = Console.ReadLine();
                Console.WriteLine("请输入文档导出的文件类型（可选值：docx、md、pdf，为空或其他非可选值则默认为docx）：");
                DocSaveType = Console.ReadLine();
                Console.WriteLine("请输入要导出的知识库Id（为空代表从所有知识库中选择）：");
                WikiSpaceId = Console.ReadLine();
                Console.WriteLine("请输入文档导出的目录位置：");
                ExportPath = Console.ReadLine();
//#endif
            }

            InitAsposeLicense();
        }

        /// <summary>
        /// 获取命令行参数值
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string GetCommandLineArg(string[] args, string parameterName, bool canNull = false)
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

            if (!canNull)
            {
                if (!found)
                {
                    Console.WriteLine($"没有找到参数：{parameterName}");
                    Console.WriteLine("请填写以下所有参数：");
                    Console.WriteLine("  --appId           飞书自建应用的AppId.");
                    Console.WriteLine("  --appSecret       飞书自建应用的AppSecret.");
                    Console.WriteLine("  --spaceId         飞书导出的知识库Id.");
                    Console.WriteLine("  --saveType        文档导出的文件类型（可选值：docx、md、pdf，为空或其他非可选值则默认为docx）.");
                    Console.WriteLine("  --exportPath      文档导出的目录位置.");
                    Environment.Exit(0);
                }

                // 参数值为空
                if (string.IsNullOrWhiteSpace(paraValue))
                {
                    Console.WriteLine($"参数{parameterName}不能为空");
                    Environment.Exit(0);
                }
            }

            return paraValue;
        }
    }
}