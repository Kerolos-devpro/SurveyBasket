using Asp.Versioning;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Health;
using SurveyBasket.Api.Settings;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api;

public static class DependencyInjection 
{

    public static IServiceCollection AddDependencies(this IServiceCollection services , IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowOrigins").Get<string[]>();
        services.AddControllers();


        services.AddHybridCache();
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
            .AddAuthConfig(configuration)
            .AddBackGroundJobsConfig(configuration);

       services.AddDbContext<ApplicationDbContext>(
               options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService , UserService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IRoleService, RoleService>();
        


        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddHttpContextAccessor();
        services.AddHealthChecks()
            .AddSqlServer(name:"database", connectionString: configuration.GetConnectionString("DefaultConnection")!)
            .AddHangfire(options =>
            {
                options.MinimumAvailableServers = 1;

            })
            .AddCheck<MailProviderHealthCheck>(name : "Mail Provider");

        services.AddRateLimitingConfig();
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;


            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
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
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

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

        services.Configure<IdentityOptions>(options =>
        {
               options.Password.RequiredLength = 8;
               options.SignIn.RequireConfirmedEmail = true;
               options.User.RequireUniqueEmail = true; 
        });
        return services;
    }
    private static IServiceCollection AddBackGroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
     .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
     .UseSimpleAssemblyNameTypeSerializer()
     .UseRecommendedSerializerSettings()
     .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }
    private static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy(RateLimiters.IpLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                    }
                )
            );

            rateLimiterOptions.AddPolicy(RateLimiters.UserLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                    }
                )
            );

            rateLimiterOptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
            {
                options.PermitLimit = 1000;
                options.QueueLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            //rateLimiterOptions.AddTokenBucketLimiter("token", options =>
            //{
            //    options.TokenLimit = 2;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            //    options.TokensPerPeriod = 2;
            //    options.AutoReplenishment = true;
            //});

            //rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            //{
            //    options.PermitLimit = 2;
            //    options.Window = TimeSpan.FromSeconds(20);
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

            //rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
            //{
            //    options.PermitLimit = 2;
            //    options.Window = TimeSpan.FromSeconds(20);
            //    options.SegmentsPerWindow = 2;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});
        });

        return services;
    }
}

