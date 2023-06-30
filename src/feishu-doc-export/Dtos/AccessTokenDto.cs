using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    public class AccessTokenDto
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        [JsonPropertyName("tenant_access_token")]
        public string TenantAccessToken { get; set; }

        public int Expire { get; set; }
    }
}
