using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OdeToFood.Data;

namespace OdeToFood
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //The Dependency Injection pattern is used heavely in ASP.NET Core architecture. It includes built-in IoC container to provide dependent objects using constructors.
        //ConfigureServices method is a place where you can register your dependent classes with the built-in IoC container.
        //ASP.NET Core injects objects of dependency classes through constructor or method by using built-in IoC container.

//        The built-in IoC container supports three kinds of lifetimes:
//Singleton: IoC container will create and share a single instance of a service throughout the application's lifetime.
//Transient: The IoC container will create a new instance of the specified service type every time you ask for it.
//Scoped: IoC container will create an instance of the specified service type once per request and will be shared in a single request.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddRazorPages();
            services.AddControllers();
            services.AddDbContextPool<OdeToFoodDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("OdeToFoodDb"));
            });
            //services.AddSingleton<IRestaurantData, InMemoryRestaurantData>();
            services.AddScoped<IRestaurantData, SqlRestaurantData>();
            services.Add(new ServiceDescriptor(typeof(ILog), new MyConsoleLogger()));
            services.Add(new ServiceDescriptor(typeof(ILog), new MyConsoleLogger()));    // singleton
            services.Add(new ServiceDescriptor(typeof(ILog), typeof(MyConsoleLogger), ServiceLifetime.Transient)); // Transient
            services.Add(new ServiceDescriptor(typeof(ILog), typeof(MyConsoleLogger), ServiceLifetime.Scoped));    // Scoped
            services.AddSingleton<ILog, MyConsoleLogger>();
            services.AddSingleton(typeof(ILog), typeof(MyConsoleLogger));

            services.AddTransient<ILog, MyConsoleLogger>();
            services.AddTransient(typeof(ILog), typeof(MyConsoleLogger));

            services.AddScoped<ILog, MyConsoleLogger>();
            services.AddScoped(typeof(ILog), typeof(MyConsoleLogger));

        }

        //A middleware is a component (class) which is executed on every request in ASP.NET Core application.
        //Request flow in and Response flow out so each piece of middleware can inspect request 
        //Each middleware adds or modifies http request and optionally passes control to the next middleware component
        //Run() is an extension method on IApplicationBuilder instance which adds a terminal middleware to the application's request pipeline.
        //The RequestDelegate is a delegate method which handles the request. The following is a RequestDelegate signature.
        //public delegate Task RequestDelegate(HttpContext context);
        //Use includes next parameter to invoke next middleware in the sequence.
        //IoC Container: It includes the built-in IoC container for automatic dependency injection which makes it maintainable and testable
        //The Configure method is a place where you can configure application request pipeline for your application using IApplicationBuilder instance that is provided by the built-in IoC container.
        //IApplicationBuilder, IHostingEnvironment, and ILoggerFactory by default.These services are framework services injected by built-in IoC container.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //Captures synchronous and asynchronous exceptions from the pipeline and generates HTML error responses.
            }
            else
            {
                app.UseExceptionHandler("/Error"); //Catch exceptions, log them and re-execute in an alternate pipeline.
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World From 2nd Middleware");
                });
                app.Run(MyMiddleware);
                app.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("Hello World From 1st Middleware!");
                    await next();
                });
                app.UseMyMiddleware();
                app.UseHsts();
            }

            app.Use(SayHelloMiddleware);
            app.UseHttpsRedirection();
            app.UseStaticFiles(); //Adds support for serving static files and directory browsing.
            app.UseNodeModules();
            // app.UseAuthentication(); -- Adds authentication support.
            //app.UseCors();--Configures Cross-Origin Resource Sharing.
            //app.UseSession();--Adds support for user session.
            // aspnetcore30
            app.UseRouting();// Adds routing capabilities for MVC or web form
            //app.UseAuthorization();
           app.UseEndpoints(e =>
            {
                e.MapRazorPages();
                e.MapControllers();
            });
        }

        private async Task MyMiddleware(HttpContext context)
        {
            await context.Response.WriteAsync("Hello World! ");
        }


        private RequestDelegate SayHelloMiddleware(
                                    RequestDelegate next)
        {
            return async ctx =>
            {

                if (ctx.Request.Path.StartsWithSegments("/hello"))
                {
                    await ctx.Response.WriteAsync("Hello, World!");
                }
                else
                {
                    await next(ctx);
                }
            };
        }
    }
}
