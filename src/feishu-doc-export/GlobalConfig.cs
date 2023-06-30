using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class GlobalConfig
    {
        public static string AppId { get; set; }

        public static string AppSecret { get; set; }

        public static string ExportPath { get; set; }

        public static string WikiSpaceId { get; set; }

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
