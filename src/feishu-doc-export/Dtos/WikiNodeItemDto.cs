using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    [Serializable]
    public class WikiNodeItemDto
    {
        [JsonPropertyName("space_id")]
        public string SpaceId { get; set; }

        [JsonPropertyName("node_token")]
        public string NodeToken { get; set; }

        [JsonPropertyName("obj_token")]
        public string ObjToken { get; set; }

        [JsonPropertyName("obj_type")]
        public string ObjType { get; set; }

        [JsonPropertyName("parent_node_token")]
        public string ParentNodeToken { get; set; }

        [JsonPropertyName("node_type")]
        public string NodeType { get; set; }

        [JsonPropertyName("origin_node_token")]
        public string OriginNodeToken { get; set; }

        [JsonPropertyName("origin_space_id")]
        public string OriginSpaceId { get; set; }

        [JsonPropertyName("has_child")]
        public bool HasChild { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("obj_create_time")]
        public string ObjCreateTime { get; set; }

        [JsonPropertyName("obj_edit_time")]
        public string ObjEditTime { get; set; }

        [JsonPropertyName("node_create_time")]
        public string NodeCreateTime { get; set; }

        public string Creator { get; set; }

        public string Owner { get; set; }
    }

    public class WikiNodePagedResult
    {
        public List<WikiNodeItemDto> Items { get; set; }

        [JsonPropertyName("page_token")]
        public string PageToken { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
