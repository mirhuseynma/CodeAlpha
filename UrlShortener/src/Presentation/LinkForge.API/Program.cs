var builder = WebApplication.CreateBuilder(args);

// Centralized DI for all layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

await app.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment()) { app.UseOpenApi(); app.UseSwaggerUi(); }

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();
