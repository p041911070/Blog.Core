﻿using Blog.Core.AuthHelper;
using Blog.Core.Common;
using Blog.Core.Extensions;
using Blog.Core.Gateway.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.V2.DependencyInjection;

namespace Blog.Core.AdminMvc
{
    public class Startup
    {
        /**
        *┌──────────────────────────────────────────────────────────────┐
        *│　描    述：模拟一个网关项目         
        *│　测    试：在网关swagger中查看具体的服务         
        *│　作    者：anson zhang                                             
        *└──────────────────────────────────────────────────────────────┘
        */
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsettings(Configuration));

            services.AddAuthentication_JWTSetup();

            services.AddAuthentication()
               .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(Permissions.GWName, _ => { });


            services.AddNacosV2Config(Configuration, null, "nacosConfig");
            services.AddNacosV2Naming(Configuration, null, "nacos");
            services.AddHostedService<ApiGateway.Helper.OcelotConfigurationTask>();


            services.AddCustomSwaggerSetup();

            services.AddControllers();

            services.AddHttpContextSetup();

            services.AddCorsSetup();

            services.AddCustomOcelotSetup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCustomSwaggerMildd();

            app.UseCors(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseMiddleware<CustomJwtTokenAuthMiddleware>();
           
            app.UseCustomOcelotMildd().Wait();
        }
    }
}
