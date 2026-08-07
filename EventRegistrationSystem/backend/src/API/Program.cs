using EventRegistrationSystem.Application.Extensions;
using EventRegistrationSystem.Infrastructure.Extensions;
using EventRegistrationSystem.Persistence.Extensions;

using EventRegistrationSystem.API.Middleware;
using DotNetEnv;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

// Clean Architecture Layers
builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

