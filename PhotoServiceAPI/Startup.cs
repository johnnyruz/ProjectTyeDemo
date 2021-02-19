using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PhotoServiceAPI.Health;
using PhotoServiceAPI.Models;
using PhotoServiceAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoServiceAPI
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
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetServiceUri("rabbitmq", binding: "rmq"), "/", h =>
                    {
                        h.Username("user");
                        h.Password("pass");
                    });
                });
            });

            services.AddMassTransitHostedService();

            services.AddHealthChecks().AddCheck<ReadinessCheck>("readiness-check", tags: new[] { "ready" });

            services.Configure<PhotosDatabaseSettings>(options => {
                options.ConnectionString = Configuration.GetConnectionString("mongo");
                options.DatabaseName = "photos";
                options.PhotosCollectionName = "photos";
            });

            services.AddSingleton<IPhotosDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<PhotosDatabaseSettings>>().Value);

            services.AddSingleton<PhotoService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhotoServiceAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhotoServiceAPI v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials()
                    .WithOrigins(Configuration.GetServiceUri("frontend", binding: "http").ToString().TrimEnd('/'),
                                 Configuration.GetServiceUri("frontendblazor", binding: "http").ToString().TrimEnd('/'));
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthy", new HealthCheckOptions() { Predicate = (check) => !check.Tags.Contains("ready") });
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("ready") });
            });
        }
    }
}
