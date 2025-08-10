using MapsterMapper;
using SurveyBasket.Api.Services;
using System.Reflection;

namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<IPollService , PollService>();

            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());


            builder.Services.AddSingleton<IMapper>(new Mapper(mappingConfig));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openApi/v1.json" , "v1"));
            }

          
            app.UseAuthorization();
           

            app.MapControllers();

            app.Run();
        }
    }
}
