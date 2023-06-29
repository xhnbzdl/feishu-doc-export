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

        static Dictionary<string, string> fileExtensionDict = new Dictionary<string, string>()
        {
            {"doc","docx" },
            {"docx","docx" },
            {"sheet","xlsx" },
            {"bitable","xlsx" },
        };

        public static string GetFileExtension(string key)
        {
            return fileExtensionDict[key];
        }
    }
}
