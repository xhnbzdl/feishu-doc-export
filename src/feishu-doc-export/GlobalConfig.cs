using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class GlobalConfig
    {
        public static string AppId { get; set; } = "cli_a4ffeeda46bc9013";

        public static string AppSecret { get; set; } = "VWqsXnY83R1p6psboMRu0evFRRnEc5kD";

        public static string ExportPath { get; set; } = "D:\\temp\\测试飞书文档";

        public static string WikiSpaceId { get; set; } = "7250298174215471107";

        /// <summary>
        /// 飞书支持导出的文件类型和导出格式
        /// </summary>
        static Dictionary<string, string> fileExtensionDict = new Dictionary<string, string>()
        {
            {"doc","docx" },
            {"docx","docx" },
            {"sheet","xlsx" },
            {"bitable","xlsx" },
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
    }
}
