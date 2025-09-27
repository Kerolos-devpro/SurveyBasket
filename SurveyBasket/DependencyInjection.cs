using Microsoft.AspNetCore.Authentication.JwtBearer;
using SurveyBasket.Api.Authentication;
using System.Text;

namespace SurveyBasket.Api;

public static class DependencyInjection 
{

    public static IServiceCollection AddDependencies(this IServiceCollection services , IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowOrigins").Get<string[]>();
        services.AddControllers();

        services.AddCors(option => 
           option.AddDefaultPolicy( builder =>
               builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins( allowedOrigins! )
           )
        );

        services.AddSwager()
            .AddMapsterConfig()
            .AddFluentValidationConfig()
            .AddAuthConfig(configuration);

       services.AddDbContext<ApplicationDbContext>(
               options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
    private static IServiceCollection AddSwager(this IServiceCollection services)
    {

        services.AddOpenApi();
        services.AddScoped<IPollService, PollService>();


        return services;
    }
    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {

        services
             .AddFluentValidationAutoValidation()
             .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider , JwtProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience= jwtSettings?.Audience
            };

        });
        return services;
    }
}

