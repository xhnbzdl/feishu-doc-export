using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class RequestData
    {
        /// <summary>
        /// 创建获取token的请求参数
        /// </summary>
        /// <param name="app_id"></param>
        /// <param name="app_secret"></param>
        /// <returns></returns>
        public static object CreateAccessToken(string app_id,string app_secret)
        {
            return new
            {
                app_id, app_secret
            };
        }
    }
}
