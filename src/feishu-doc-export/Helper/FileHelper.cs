using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void Save(this string path, byte[] content)
        {
            var dir = Path.GetDirectoryName(path);

            dir.CreateIfNotExist();

            File.WriteAllBytes(path, content);
        }

        /// <summary>
        /// 如果目录不存在，那么创建目录
        /// </summary>
        /// <param name="path"></param>
        public static void CreateIfNotExist(this string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
