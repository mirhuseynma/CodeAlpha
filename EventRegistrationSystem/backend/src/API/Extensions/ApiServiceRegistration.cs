namespace EventRegistrationSystem.API.Extensions;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        
        services.AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Vite and React default ports
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Sadəcə JWT tokenini daxil edin. 'Bearer ' prefiksi avtomatik əlavə edilir."
            });

            c.OperationFilter<SecurityRequirementsOperationFilter>();

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        // Setup Identity Core in the API layer where it belongs
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.SignIn.RequireConfirmedEmail = false; // Disabled for local testing
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventRegistrationSystem.Persistence.Context.AppDbContext>();
        
        await DatabaseSeeder.SeedAsync(roleManager, userManager, configuration, dbContext);
    }
}
