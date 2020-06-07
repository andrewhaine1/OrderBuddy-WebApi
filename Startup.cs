using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Ord.WebApi.Data.Contexts;
using Ord.WebApi.Mappers.Menu;
using Ord.WebApi.Mappers.Order;
using Ord.WebApi.Mappers.Restaurant;
using Ord.WebApi.Services.Data.Areas;
using Ord.WebApi.Services.Data.Menus;
using Ord.WebApi.Services.Data.Orders;
using Ord.WebApi.Services.Data.Restaurants;
using Ord.WebApi.Services.Data.Shared;
using Ord.WebApi.Services.Data.User;
using Ord.WebApi.Services.Web.Firebase;

namespace Ord.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
	    Environment = environment;
        }

        public IConfiguration Configuration { get; }
	public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure CORS so the API allows requests from JavaScript.  
            // For demo purposes, all origins/headers/methods are allowed.  
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOriginsHeadersAndMethods",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Default")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // base-address of your identityserver
                options.Authority = "http://localhost:5000";

                // name of the API resource
                options.Audience = "orderbuddyapi";

                options.RequireHttpsMetadata = false;
            });

            services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IOrdUserService, OrdUserService>();
            services.AddScoped<IServiceAreaService, ServiceAreaService>();

            services.AddScoped<IRestaurantMapper, RestaurantMapper>();
            services.AddScoped<IMenuMapper, MenuMapper>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderMapper, OrderMapper>();
            services.AddScoped<IFcmService, FcmService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ApplicationDbContext context, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //context.CreateTestData();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async errContext =>
                    {
                        var exceptionHandlerFeature = errContext.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global Exception Logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        errContext.Response.StatusCode = 500;
                        await errContext.Response.WriteAsync("An unexpected fault occured. Try again later");
                    });
                });

            }

	    app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            // Enable CORS
            app.UseCors("AllowAllOriginsHeadersAndMethods");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "media")),
                RequestPath = "/media"
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
