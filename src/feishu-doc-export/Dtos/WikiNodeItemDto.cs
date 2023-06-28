using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    [Serializable]
    public class WikiNodeItemDto
    {
        public string SpaceId { get; set; }
        public string NodeToken { get; set; }
        public string ObjToken { get; set; }
        public string ObjType { get; set; }
        public string ParentNodeToken { get; set; }
        public string NodeType { get; set; }
        public string OriginNodeToken { get; set; }
        public string OriginSpaceId { get; set; }
        public bool HasChild { get; set; }
        public string Title { get; set; }
        public string ObjCreateTime { get; set; }
        public string ObjEditTime { get; set; }
        public string NodeCreateTime { get; set; }
        public string Creator { get; set; }
        public string Owner { get; set; }
    }

    public class WikiNodePagedResult
    {
        public List<WikiNodeItemDto> Items { get; set; }

        public string PageToken { get; set; }

        public bool HasMore { get; set; }
    }
}
