using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export.Dtos
{
    public class ResponseData : ResponseData<object>
    {
    }

    public class ResponseData<T>
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public virtual T Data { get; set; }
    }
}
