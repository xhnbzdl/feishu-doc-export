using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    public class ExportTaskInfoDto
    {
        [JsonPropertyName("file_extension")]
        public string FileExtension { get; set; }

        public string Type { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("file_token")]
        public string FileToken { get; set; }

        [JsonPropertyName("file_size")]
        public long FileSize { get; set; }

        [JsonPropertyName("job_error_msg")]
        public string JobErrorMsg { get; set; }

        [JsonPropertyName("job_status")]
        public int JobStatus { get; set; }

    }

    public class ExportTaskResultDto
    {
        public ExportTaskInfoDto Result { get; set; }
    }
}
