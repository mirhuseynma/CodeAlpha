using Microsoft.AspNetCore.Identity;
using LinkForge.Application;
using LinkForge.Infrastructure;
using LinkForge.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Centralized DI for all layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddIdentity<LinkForge.Domain.Entities.AppUser, LinkForge.Domain.Entities.Role>()
    .AddEntityFrameworkStores<LinkForge.Persistence.Contexts.AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<LinkForge.Application.Common.Interfaces.ICurrentUserService, LinkForge.API.Services.CurrentUserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
