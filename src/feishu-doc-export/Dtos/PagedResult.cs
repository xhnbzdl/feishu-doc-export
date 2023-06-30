using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }

        [JsonPropertyName("page_token")]
        public string PageToken { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
