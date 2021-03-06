using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using authsvc.ServiceInterface;
using ServiceStack.Script;
using ServiceStack.Web;
using System;
using ServiceStack.Text;
using ServiceStack.Logging;

namespace authsvc
{
    public class Startup : ModularStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("authsvc", typeof(MyServices).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                AddRedirectParamsToQueryString = true,
                EmbeddedResourceBaseTypes = { typeof(ServiceStack.Desktop.DesktopAssets) },
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), HostingEnvironment.IsDevelopment()),
            });

            if (Config.DebugMode)
            {
                Plugins.Add(new HotReloadFeature {
                    VirtualFiles = VirtualFiles, //Monitor all folders for changes including /src & /wwwroot
                });
            }
            
            // enable server-side rendering, see: https://sharpscript.net/docs/sharp-pages
            Plugins.Add(new SharpPagesFeature {
                EnableSpaFallback = true,
            }); 
        }
    }
}
