using feishu_doc_export.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace feishu_doc_export.HttpApi
{
    [HttpHost(FeiShuConsts.OpenApiEndPoint)]
    public interface IFeiShuHttpApi : IHttpApi
    {
        /// <summary>
        /// 获取自建应用的AccessToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/open-apis/auth/v3/tenant_access_token/internal")]
        Task<AccessTokenDto> GetTenantAccessToken(object request);

        /// <summary>
        /// 获取所有的知识库
        /// </summary>
        /// <returns></returns>
        [HttpGet("/open-apis/wiki/v2/spaces")]
        [OAuthToken]
        [JsonReturn]
        Task<ResponseData<PagedResult<WikiSpaceDto>>> GetWikiSpaces();

        /// <summary>
        /// 获取知识库详细信息
        /// </summary>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        [HttpGet("/open-apis/wiki/v2/spaces/{spaceId}")]
        [OAuthToken]
        [JsonReturn]
        Task<ResponseData<WikiSpaceInfo>> GetWikiSpaceInfo(string spaceId);

        /// <summary>
        /// 获取知识空间子节点列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OAuthToken]
        [JsonReturn]
        Task<ResponseData<PagedResult<WikiNodeItemDto>>> GetWikeNodeList([Uri] string url);

        /// <summary>
        /// 创建文档导出任务结果
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/open-apis/drive/v1/export_tasks")]
        [OAuthToken]
        [JsonReturn]
        Task<ResponseData<ExportOutputDto>> CreateExportTask([JsonContent] object request);

        /// <summary>
        /// 查询导出任务
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("/open-apis/drive/v1/export_tasks/{ticket}?token={token}")]
        [OAuthToken]
        [JsonReturn]
        Task<ResponseData<ExportTaskResultDto>> QueryExportTask(string ticket, string token);

        /// <summary>
        /// 下载导出文件
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("/open-apis/drive/v1/export_tasks/file/{fileToken}/download")]
        [OAuthToken]
        Task<byte[]> DownLoad(string fileToken);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileToken"></param>
        /// <returns></returns>
        [HttpGet("/open-apis/drive/v1/files/{fileToken}/download")]
        [OAuthToken]
        Task<byte[]> DownLoadFile(string fileToken);
    }
}
