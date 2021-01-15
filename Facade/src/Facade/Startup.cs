using Facade.Data;
using Facade.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddHttpContextAccessor();
            services.AddScoped<FacadeSession>();
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                
                endpoints.MapReverseProxy();
            });
        }
    }
}
