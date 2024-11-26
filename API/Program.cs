using API.Helpers;
using API.Middlewares;
using Application.Commands;
using Application.Configurations;
using Application.Configurations.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(typeof(ApiExceptionFilter));
    opt.Filters.Add(typeof(ValidationExceptionFilter));
}).AddNewtonsoftJson();

builder.Services.AddValidatorsFromAssemblyContaining<BaseCommand>();
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDatabaseConfiguration, DatabaseConfiguration>();
var databaseConfig = builder.Services.BuildServiceProvider().GetRequiredService<IDatabaseConfiguration>();
builder.Services.AddDatabaseSetup(databaseConfig);
builder.Services.AddAutoMapperSetup();
builder.Services.AddDependencyInjector();
builder.Services.AddJwtConfig(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerSetup();
}

app.UseCors(policy =>
    policy.WithOrigins()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
);

app.UseHttpsRedirection();

app.UseMiddleware<ForbiddenMiddleware>();

app.UseAuthorization();

app.MapControllers();

await app.SeedRolesAndClaims();

app.Run();