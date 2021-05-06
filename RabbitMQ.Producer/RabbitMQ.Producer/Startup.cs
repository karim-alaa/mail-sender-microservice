using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Producer.Data;
using RabbitMQ.Producer.Dtos.Config;
using RabbitMQ.Producer.Services;
using System;
using System.IO;
using RabbitMQ.Producer.Utilities;
using Quartz.Spi;
using Quartz.Impl;
using Quartz;
using RabbitMQ.Producer.Jobs;
using RechargeAutoAction.Jobs.ManageRechargeCases;
using Serilog;
using System.Linq;
using RabbitMQ.Producer.Constants;
using RabbitMQ.Producer.Middlewares.Logging;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Producer.Queues.RabbitMQ;

namespace RabbitMQ.Producer
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        private AppConfig _config;
        private CustomRabbitMQ CustomRabbitMQ;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetAppSettingsFile();

            _config = Configuration.Get<AppConfig>();
            

            services.AddControllers();
            
            services.AddAutoMapper(new Type[] { typeof(AutoMapperProfile) });

            services.AddDbContext<DataContext>
                    (options => options.UseSqlServer(_config.DBConfig.ConnectionString));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RabbitMQ.Producer", Version = "v1" });
            });

            // Add Quartz services
            services.AddTransient<IJobFactory, SingletonJobFactory>();
            services.AddTransient<ISchedulerFactory, StdSchedulerFactory>();

            // Add HostedService
            services.AddHostedService<QuartzHostedService>();

            // Add our job
            services.AddTransient<HandleReDeliveryMessages>();
            services.AddSingleton(new JobSchedule(jobType: typeof(HandleReDeliveryMessages),
                cronExpression: _config.ReDelivery.JobRunTime));

            services.AddTransient<IQueueService, QueueService>();
            services.AddTransient<IReDeliveryService, ReDeliveryService>();
            services.AddSingleton<ICustomLoggingConfig, CustomLoggingConfig>();
           
            services.AddSingleton(_config);


            CustomRabbitMQ = new CustomRabbitMQ(_config, services.BuildServiceProvider());
            services.AddSingleton(CustomRabbitMQ);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICustomLoggingConfig customLoggingConfig)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RabbitMQ.Producer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.Use(async (context, next) => {
                context.Request.EnableBuffering();
                await next();
            });

            // Logging and debugging
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // setup custom logging
            Log.Logger = new LoggerConfiguration()
            .WriteTo.MSSqlServer(
              connectionString: _config.CustomLogging.WriteTo.FirstOrDefault().Args.ConnectionString,
              sinkOptions: customLoggingConfig.GetSinkOpts(),
              columnOptions: customLoggingConfig.GetColumnOptions())
            .MinimumLevel.Information()
            .Enrich.WithMachineName()
            .CreateLogger();
        }


        // Add appsettings files
        private void SetAppSettingsFile()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{env}.json", true, true);
            Configuration = builder.Build();
        }
    }
}
