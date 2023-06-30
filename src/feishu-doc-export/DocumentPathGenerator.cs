using feishu_doc_export.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class DocumentPathGenerator
    {
        private static Dictionary<string, string> documentPaths;

        public static void GenerateDocumentPaths(List<WikiNodeItemDto> documents, string rootFolderPath)
        {
            documentPaths = new Dictionary<string, string>();

            var topDocument = documents.Where(x => string.IsNullOrWhiteSpace(x.ParentNodeToken));
            foreach (var document in topDocument)
            {
                GenerateDocumentPath(document, rootFolderPath, documents);
            }

        }

        private static void GenerateDocumentPath(WikiNodeItemDto document, string parentFolderPath, List<WikiNodeItemDto> documents)
        {
            // 替换文件名中的非法字符
            string title = Regex.Replace(document.Title, @"[\\/:\*\?""<>\|]", "-");
            string documentFolderPath = Path.Combine(parentFolderPath, title);

            documentPaths[document.ObjToken] = documentFolderPath;

            foreach (var childDocument in GetChildDocuments(document, documents))
            {
                GenerateDocumentPath(childDocument, documentFolderPath, documents);
            }
        }

        private static IEnumerable<WikiNodeItemDto> GetChildDocuments(WikiNodeItemDto document, List<WikiNodeItemDto> documents)
        {
            return documents.Where(d => d.ParentNodeToken == document.NodeToken);
        }

        public static string GetDocumentPath(string objToken)
        {
            documentPaths.TryGetValue(objToken, out string path);
            return path;
        }
    }
}
