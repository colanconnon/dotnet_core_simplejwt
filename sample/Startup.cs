using dotnet_core_simple_jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Models;
using Microsoft.EntityFrameworkCore;


namespace Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            
            var sqlConnectionString = Configuration["Data:DbContext:ConnectionString"];
            services.AddDbContext<SampleDbContext>(options =>
               options.UseNpgsql(
                   sqlConnectionString
               )
            );
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SampleDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<JwtMiddlewareOptions>(options =>
            {
                options.secret = "blahblahblah";
                options.tokenEndpoint = "/login";
            });
            services.AddScoped<AuthorizationAttribute>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseJwt<ApplicationUser>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // routes.MapSpaFallbackRoute(
                //     name: "spa-fallback",
                //     defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
