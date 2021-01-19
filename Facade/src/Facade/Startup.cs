using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Facade.Data;
using Facade.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Microsoft.ReverseProxy.Service.Proxy;

namespace Facade
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFacadeSession();
            services.AddScoped<ShoppingCart>();

            services.AddAuthentication("Facade")
                .AddScheme<FacadeAuthenticationOptions, FacadeAuthenticationHandler>("Facade",
                    o => Configuration.GetSection("FacadeAuthentication").Bind(o));
            
            services.AddDbContextPool<WingtipToysContext>(builder =>
            {
                builder.UseSqlServer(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\aspnet\\Wingtip-Toys\\WingtipToys\\App_Data\\wingtiptoys.mdf;Integrated Security=True");
            });

            services.AddControllersWithViews();
            services.AddReverseProxy() 
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpProxy httpProxy)
        {
            var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false
            });
            var transformer = new RedirectTransformer();
            var requestOptions = new RequestProxyOptions(TimeSpan.FromSeconds(100), null);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                endpoints.Map("/{**catch-all}", async httpContext =>
                {
                    await httpProxy.ProxyAsync(httpContext, "http://localhost:24019/", httpClient, requestOptions, transformer);
                    // var errorFeature = httpContext.Features.Get<IProxyErrorFeature>();
                    // if (errorFeature != null)
                    // {
                    //     var error = errorFeature.Error;
                    //     var exception = errorFeature.Exception;
                    // }
                });
            });
        }

    }
        internal class RedirectTransformer : HttpTransformer
        {
            public override async Task TransformResponseAsync(HttpContext context, HttpResponseMessage response)
            {
                if (response.Headers.Location?.IsAbsoluteUri == true)
                {
                    var relative = response.Headers.Location.PathAndQuery;
                    response.Headers.Location = new Uri(relative, UriKind.Relative);
                }
                await base.TransformResponseAsync(context, response);
            }
        }
}
