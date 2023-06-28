
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using WebApiClientCore;

namespace feishu_doc_export
{
    internal class Program
    {
        static IFeiShuHttpApi feiShuHttpApi;

        static void Main(string[] args)
        {
            IOC.Init();

            feiShuHttpApi = IOC.IoContainer.GetService<IFeiShuHttpApi>();

            var wikiNodes = GetWikiNodeList("7086342166150250500");
            Console.WriteLine(wikiNodes);

        }

        private async Task<string> GetTenantAccessToken()
        {
            var requestData = RequestData.CreateAccessToken("", "");

            var result = await feiShuHttpApi.GetTenantAccessToken(requestData);

            return result["tenant_access_token"].ToString();
        }

        static object GetWikiNodeList(string spaceId, string pageToken = null, string parentNodeToken = null)
        {
            StringBuilder urlBuilder = new StringBuilder($"{FeiShuConsts.OpenApiEndPoint}/open-apis/wiki/v2/spaces/{spaceId}/nodes?page_size=50");
            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                urlBuilder.Append($"&page_token={pageToken}");
            }

            if (!string.IsNullOrWhiteSpace(parentNodeToken))
            {
                urlBuilder.Append($"&parent_node_token={parentNodeToken}");
            }

            var parentNodes = feiShuHttpApi.GetWikeNodeList(urlBuilder.ToString()).Result;
            return parentNodes;
        }
    }
}