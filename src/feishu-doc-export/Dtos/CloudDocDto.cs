using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    /// <summary>
    /// 个人空间云文档
    /// </summary>
    public class CloudDocDto
    {
        [JsonPropertyName("created_time")]
        public string CreatedTime { get; set; }

        [JsonPropertyName("modified_time")]
        public string ModifiedTime { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("owner_id")]
        public string OwnerId { get; set; }

        [JsonPropertyName("parent_token")]
        public string ParentToken { get; set; }

        public string Token { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }
    }
}
