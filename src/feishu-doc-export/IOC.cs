using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class IOC
    {
        public static IServiceProvider IoContainer { get; private set; }

        /// <summary>
        /// 创建服务注册器
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <summary>
        /// 生成Ioc容器
        /// </summary>
        /// <param name="services"></param>
        public static void BuildIoContainer(this IServiceCollection services)
        {
            IoContainer = services.BuildServiceProvider();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            CreateServiceCollection().ConfigService().BuildIoContainer();

        }

        /// <summary>
        /// 添加RabbitMq服务注册
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection ConfigService(this IServiceCollection services)
        {
            services.AddHttpApi<IFeiShuHttpApi>();

            services.AddClientCredentialsTokenProvider<IFeiShuHttpApi>(o =>
            {
                o.Endpoint = new Uri("https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal");
                o.Credentials.Client_id = "cli_a4ffeeda46bc9013";
                o.Credentials.Client_secret = "VWqsXnY83R1p6psboMRu0evFRRnEc5kD";
            });
            

            return services;
        }
    }
}
