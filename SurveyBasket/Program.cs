
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

            app.UseCors();
          
            app.UseAuthorization();

            app.UseOutputCache();

            app.MapControllers();
            app.UseExceptionHandler();
            app.Run();
        }
    }
}
