using API.Helpers;
using Application.Configurations;
using Application.Configurations.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
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

app.UseAuthorization();

app.MapControllers();

await app.SeedRolesAndClaims();

app.Run();