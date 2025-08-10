

namespace SurveyBasket.Api;

public static class DependencyInjection 
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {

        services.AddControllers();

        services.AddSwager()
            .AddMapsterConf()
            .AddFluentValidationConf();

        services.AddScoped<IPollService, PollService>();

        return services;
    }
    public static IServiceCollection AddSwager(this IServiceCollection services)
    {

        services.AddOpenApi();
        services.AddScoped<IPollService, PollService>();


        return services;
    }
    public static IServiceCollection AddMapsterConf(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        services
             .AddFluentValidationAutoValidation()
             .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddFluentValidationConf(this IServiceCollection services)
    {

        services
             .AddFluentValidationAutoValidation()
             .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}

