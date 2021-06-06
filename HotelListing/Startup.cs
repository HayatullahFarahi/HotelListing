using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Middleware;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelListing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );

            // TODO: AUTH
            //Referencing ServiceExtensions file for identityUser service
            //ConfigureIdentity method of service extension
            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJWT(Configuration);
            
            services.AddCors(options => {
                options.AddPolicy("AllowAll", builder => 
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                    );
            });
            
            //adding the auto mapper initializer class
            services.AddAutoMapper(typeof(MapperInitializer));
            //TODO: registering IUnitOfWork service 
            //AddScoped , AddTransient, AddSingleton
                // AddTransient: everytime it is need a new instance will be created 
                // AddScoped: a single instance is created for a lifetime of a certain request
                // AddSingleton: a single instance is created for the entire duration of the application
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //TODO: AUTH
            services.AddScoped<IAuthManager, AuthManager>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });
            
            //TODO: AddNewtonsoftJson to ignore the loop between country and the hotel models 
            services.AddControllers().AddNewtonsoftJson(op =>
                op.SerializerSettings.ReferenceLoopHandling = 
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            
            //TODO: adding API version service
            services.ConfigureVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // provides the documentation for API users
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));
            
            //TODO: using global error handler
            app.ConfigurationExceptionHandler();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthentication();
            //TODO: this middleware checks for expiration token and returns custom status and response
            app.UseMiddleware<ExpiredTokenMiddleware>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // TODO: defining convention based routing for MVC Web App
                // endpoints.MapControllerRoute(
                //     name: "default",
                //     pattern: "{controller=Home}/{action=Index}/{id}"
                // );
                endpoints.MapControllers();
            });
        }
    }
}
