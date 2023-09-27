using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    /// <summary>
    /// 个人空间云文档文件夹信息
    /// </summary>
    public class CloudDocFolderMeta
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }

        public string CreateUid { get; set; }

        public string EditUid { get; set; }

        public string ParentId { get; set; }

        public string OwnUid { get; set; }
    }
}
