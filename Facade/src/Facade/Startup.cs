using System;
using System.Net;
using System.Net.Http;
using Facade.Data;
using Facade.LibraryStuff.Authentication;
using Facade.LibraryStuff.Proxy;
using Facade.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            services.AddAuthentication(FacadeAuthenticationDefaults.Scheme)
                .AddFacade();
            
            services.AddDbContextPool<WingtipToysContext>(builder =>
            {
                builder.UseSqlServer(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\aspnet\\Wingtip-Toys\\WingtipToys\\App_Data\\wingtiptoys.mdf;Integrated Security=True");
            });

            services.AddControllersWithViews();
            services.AddReverseProxy() 
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
            // {
            //     UseProxy = false,
            //     AllowAutoRedirect = false,
            //     AutomaticDecompression = DecompressionMethods.None,
            //     UseCookies = false
            // });
            // var transformer = new RedirectTransformer();
            // var requestOptions = new RequestProxyOptions(TimeSpan.FromSeconds(100), null);
            //
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

                endpoints.MapFacadeProxy();
                //
                // endpoints.Map("/{**catch-all}", async httpContext =>
                // {
                //     await httpProxy.ProxyAsync(httpContext, "http://localhost:24019/", httpClient, requestOptions, transformer);
                // });
            });
        }

    }
}
