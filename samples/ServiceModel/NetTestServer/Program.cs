using System;
using Contracts;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NetTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel(options => { options.ListenLocalhost(8080); })
                   .UseStartup<Startup>();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(builder =>
            {
                builder
                    .AddService<TestService>()
                    .AddServiceEndpoint<TestService, ITestService>(new BasicHttpBinding(), "/basic");
            });
        }
    }

    public class TestService : ITestService
    {
        public string SuccessOperation(string message) => message;

        public string FaultyOperation(string message) => throw new Exception();
    }
}
