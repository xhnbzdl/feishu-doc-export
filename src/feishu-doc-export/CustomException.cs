using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    internal class CustomException:Exception
    {
        public int Code { get; set; }

        public CustomException(string message) :base(message)
        {
            
        }

        public CustomException(string message,int code) : base(message)
        {
            Code = code;
        }
    }
}
