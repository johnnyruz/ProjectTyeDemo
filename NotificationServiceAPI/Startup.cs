using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NotificationServiceAPI.Hubs;
using NotificationServiceAPI.Messaging;

namespace NotificationServiceAPI
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
                x.AddConsumer<CommentAddedEventConsumer>();
                x.AddConsumer<PhotoDeletedEventConsumer>();
                x.AddConsumer<PhotoAddedEventConsumer>();
                x.AddConsumer<ProcessingCompleteEventConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetServiceUri("rabbitmq", binding: "rmq"), "/", h =>
                    {
                        h.Username("user");
                        h.Password("pass");
                    });
                    cfg.ReceiveEndpoint("signalr-service-listener", e =>
                    {
                        e.ConfigureConsumer<CommentAddedEventConsumer>(context);
                        e.ConfigureConsumer<PhotoAddedEventConsumer>(context);
                        e.ConfigureConsumer<PhotoDeletedEventConsumer>(context);
                        e.ConfigureConsumer<ProcessingCompleteEventConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();
            services.AddCors();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                endpoints.MapHub<PhotoTyeHub>("/phototyehub");
            });
        }
    }
}
