using ECommerce.Infrastructure.Data.Seeding;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ECommerce.Application;
using ECommerce.Infrastructure;
using Microsoft.OpenApi;
using Serilog;
using Scalar.AspNetCore;
using ECommerce.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>{
    options.SwaggerDoc("v1",new OpenApiInfo{
        Title="E-Commerce API",
        Version="v1"
    });
    options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme{
        Type=SecuritySchemeType.Http,
        Description="Enter JWT token",
        Scheme="Bearer",
        BearerFormat="JWT",
        In=ParameterLocation.Header
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddApplication();        // MediatR, FluentValidation, Mapster
builder.Services.AddInfrastructure(builder.Configuration);  // DbContext, Identity, Services


var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    await IdentitySeeder.SeedAsync(userManager, roleManager);
}

if (app.Environment.IsDevelopment()||app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Cortexa API")
            .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    }
    );
}
app.UseSerilogRequestLogging();
//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
