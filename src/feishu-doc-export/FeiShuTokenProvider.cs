using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace feishu_doc_export
{
    public class FeiShuTokenProvider : TokenProvider
    {
        private readonly IFeiShuHttpApi _feiShuHttpApi;

        public FeiShuTokenProvider(IServiceProvider services) : base(services)
        {
            _feiShuHttpApi = services.GetService<IFeiShuHttpApi>();
        }

        protected override async Task<TokenResult> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            return await this.RequestTokenAsync(serviceProvider);
        }

        protected override async Task<TokenResult> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            var tokenResult = new TokenResult();

            var requestData = RequestData.CreateAccessToken("cli_a4ffeeda46bc9013", "VWqsXnY83R1p6psboMRu0evFRRnEc5kD");
            var result = await _feiShuHttpApi.GetTenantAccessToken(requestData);

            tokenResult.Access_token = result["tenant_access_token"].ToString();
            tokenResult.Refresh_token = tokenResult.Access_token;

            return tokenResult;
        }
    }
}
