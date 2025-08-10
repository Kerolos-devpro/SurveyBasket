
namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDependencies();

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
