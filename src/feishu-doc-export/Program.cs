
using feishu_doc_export.Dtos;
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

            var wikiNodes = GetWikiNodeList("7086342166150250500", parentNodeToken: "wikcnuGeC3ChV3TDglYUkzt7T1c").Result;

            wikiNodes = GetWikiChildNodeList("7086342166150250500", wikiNodes).Result;
            Console.WriteLine(wikiNodes.Count);

        }

        static async Task<List<WikiNodeItemDto>> GetWikiNodeList(string spaceId, string pageToken = null, string parentNodeToken = null)
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

            var resultData = await feiShuHttpApi.GetWikeNodeList(urlBuilder.ToString());

            List<WikiNodeItemDto> nodes = resultData.Data.Items;

            if (resultData.Data.HasMore && !string.IsNullOrWhiteSpace(resultData.Data.PageToken))
            {
                var moreData = await GetWikiNodeList(spaceId, resultData.Data.PageToken);
                nodes.AddRange(moreData);
            }

            return nodes;
        }

        static async Task<List<WikiNodeItemDto>> GetWikiChildNodeList(string spaceId, List<WikiNodeItemDto> wikiNodes)
        {
            List<WikiNodeItemDto> newNodes = new List<WikiNodeItemDto>();

            foreach (var item in wikiNodes)
            {
                newNodes.Add(item);
                if (item.HasChild)
                {
                    var childNodes = await GetWikiNodeList(spaceId, parentNodeToken: item.NodeToken);
                    childNodes.AddRange(await GetWikiChildNodeList(spaceId, childNodes));
                    newNodes.AddRange(childNodes);
                }
            }
            return newNodes;
        }
    }
}