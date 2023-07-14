using System.Text.RegularExpressions;

namespace feishu_doc_export.Helper
{
    public static class DocxToMdFormatHelper
    {
        /// <summary>
        /// 替换图片引用路径
        /// </summary>
        /// <param name="markdownContent"></param>
        /// <param name="currentDocPath"></param>
        /// <returns></returns>
        public static string ReplaceImagePath(this string markdownContent, string currentDocPath)
        {
            // 正则表达式匹配图片引用语法 ![...](...)
            var regex = new Regex(@"!\[.*?\]\((.*?)\)", RegexOptions.IgnoreCase);

            var replacedContent = regex.Replace(markdownContent, match =>
            {
                var imagePath = match.Groups[1].Value;

                // 如果图片引用路径是绝对路径，则将其替换为相对路径
                if (Path.IsPathRooted(imagePath))
                {
                    var relativePath = Path.GetRelativePath(Path.GetDirectoryName(currentDocPath), imagePath);
                    return $"![...]({relativePath})";
                }

                return match.Value;
            });

            return replacedContent;
        }

        /// <summary>
        /// 替换文档引用路径
        /// </summary>
        /// <param name="markdownContent"></param>
        /// <returns></returns>
        public static string ReplaceDocRefPath(this string markdownContent, string currentDocPath)
        {
            // 正则表达式匹配飞书文档的引用语法[](https://*.feishu.cn/wiki/nodeToken)
            var regex = new Regex(@"\[(?<linkText>[^\]]+)\]\((https?://[^/]+\.feishu\.cn/wiki/(?<nodeToken>[^)]+))\)");

            var replacedContent = regex.Replace(markdownContent, match =>
            {
                var fileExt = Path.GetExtension(currentDocPath);

                var nodeToken = match.Groups["nodeToken"].Value;
                var linkText = match.Groups["linkText"].Value;

                // 所引用的文档是否存在（如：非当前知识库的文档）
                var refDocPath = DocumentPathGenerator.GetDocumentPathByNodeToken(nodeToken);
                if (!string.IsNullOrWhiteSpace(refDocPath))
                {
                    var relativePath = Path.GetRelativePath(Path.GetDirectoryName(currentDocPath), refDocPath);
                    return $"[{linkText}]({relativePath}{fileExt})";
                }

                return match.Value;
            });

            return replacedContent;
        }

        /// <summary>
        /// 将docx的代码块替换为markdown语法
        /// </summary>
        /// <param name="markdownContent"></param>
        /// <returns></returns>
        public static string ReplaceCodeToMdFormat(this string markdownContent)
        {
            string pattern = @"\|(?<content>[^\n]+)\n\|\s*:\s*-\s*\|";

            string replacedContent = Regex.Replace(markdownContent, pattern, match =>
            {
                string replacement = match.Groups["content"].Value.Replace("<br>", "\n");

                replacement = replacement.Remove(replacement.LastIndexOf('|'), 1);
                replacement = replacement.Replace("`", "");

                return $"```{replacement}```";
            });

            return replacedContent;
        }
    }
}
