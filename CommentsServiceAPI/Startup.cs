using CommentsServiceAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PhotoServiceAPI.Models;
using MassTransit;
using CommentsServiceAPI.Messaging;

namespace CommentsServiceAPI
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
                x.AddConsumer<PhotoDeletedEventConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetServiceUri("rabbitmq", binding: "rmq"), "/", h =>
                    {
                        h.Username("user");
                        h.Password("pass");
                    });
                    cfg.ReceiveEndpoint("comment-service-listener", e =>
                    {
                        e.ConfigureConsumer<PhotoDeletedEventConsumer>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();

            services.Configure<CommentsDatabaseSettings>(options => {
                options.ConnectionString = Configuration.GetConnectionString("mongo");
                options.DatabaseName = "comments";
                options.CommentsCollectionName = "comments";
            });

            services.AddSingleton<ICommentsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<CommentsDatabaseSettings>>().Value);

            services.AddSingleton<CommentsService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommentsServiceAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommentsServiceAPI v1"));
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
            });
        }
    }
}
