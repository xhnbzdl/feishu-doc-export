using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    public class WikiSpaceDto
    {
        public string Description { get; set; }

        public string Name { get; set; }
        
        [JsonPropertyName("space_id")]
        public string Spaceid { get; set; }

        [JsonPropertyName("space_type")]
        public string SpaceType { get; set; }

        public string Visibility { get; set; }
    }
}
