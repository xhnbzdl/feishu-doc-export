using feishu_doc_export.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace feishu_doc_export
{
    //[HttpHost(FeiShuConsts.OpenApiEndPoint)]
    public interface IFeiShuHttpApi: IHttpApi
    {
        /// <summary>
        /// 获取自建应用的AccessToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("open-apis/auth/v3/tenant_access_token/internal")]
        Task<Dictionary<string,object>> GetTenantAccessToken(object request);

        /// <summary>
        /// 获取知识空间子节点列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<ResponseData<WikiNodePagedResult>> GetWikeNodeList([Uri] string url);
    }
}
