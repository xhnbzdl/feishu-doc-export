using feishu_doc_export.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class CloudDocPathGenerator
    {
        /// <summary>
        /// 文档token和路径的映射
        /// </summary>
        private static Dictionary<string, string> documentPaths;

        public static void GenerateDocumentPaths(List<CloudDocDto> documents, string rootFolderPath)
         {
            documentPaths = new Dictionary<string, string>();

            foreach (var document in documents)
            {
                if (!documentPaths.ContainsKey(document.Token))
                {
                    GenerateDocumentPath(document, rootFolderPath, documents);
                }
            }

        }

        private static void GenerateDocumentPath(CloudDocDto document, string parentFolderPath, List<CloudDocDto> documents)
        {
            // 替换文件名中的非法字符
            string name = Regex.Replace(document.Name, @"[\\/:\*\?""<>\|]", "-");
            string documentFolderPath = Path.Combine(parentFolderPath, name);

            documentPaths[document.Token] = documentFolderPath;

            foreach (var childDocument in GetChildDocuments(document, documents))
            {
                GenerateDocumentPath(childDocument, documentFolderPath, documents);
            }
        }

        private static IEnumerable<CloudDocDto> GetChildDocuments(CloudDocDto document, List<CloudDocDto> documents)
        {
            return documents.Where(d => d.ParentToken == document.Token);
        }

        /// <summary>
        /// 获取文档的存储路径
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        public static string GetDocumentPath(string objToken)
        {
            documentPaths.TryGetValue(objToken, out string path);
            return path;
        }
    }
}
