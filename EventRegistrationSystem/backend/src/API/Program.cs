Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Centralized DI for all layers
builder.Services
    .AddApiServices()
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.EnablePersistAuthorization();
        c.UseRequestInterceptor(
            "(request) => { " +
            "  var auth = JSON.parse(localStorage.getItem('authorized') || '{}'); " +
            "  var bearer = auth && auth.Bearer && auth.Bearer.value; " +
            "  if (bearer) { request.headers['Authorization'] = 'Bearer ' + bearer; } " +
            "  return request; " +
            "}"
        );
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();

