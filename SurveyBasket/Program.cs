 
using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Serilog;

namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDependencies(builder.Configuration);

            builder.Host.UseSerilog((context, configuration) =>
             configuration.ReadFrom.Configuration(context.Configuration)
            );

            builder.Services.AddOutputCache(
                options => options.AddPolicy
                ("Polls" ,
                x => x.Cache().Expire(TimeSpan.FromSeconds(120))
                .Tag("availableQuestion")
                )
                
            );
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openApi/v1.json" , "v1"));
            }
            app.UseSerilogRequestLogging();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization =
                [
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
                        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
                    }
                ],
                DashboardTitle = "Survey Basket Dashboard",
                //IsReadOnlyFunc = (DashboardContext context) => true
            });

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            RecurringJob.AddOrUpdate("SendNewPollNotifications", () => notificationService.SendNewPollNotifications(null) , Cron.Daily);
            app.UseCors();
          
            app.UseAuthorization();

            app.UseOutputCache();

            app.MapControllers();
            app.UseExceptionHandler();

            app.MapHealthChecks("health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
