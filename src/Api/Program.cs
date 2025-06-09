using System.Net;
using System.Text;
using Api.Configuration;
using Application;
using Infrastructure;
using Infrastructure.Database;
using Api.ExceptionHandlers;
using Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

var logPattern =
    @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [ClientIp={ClientIp}] [CorrelationId={CorrelationId}] {Message:lj}{NewLine}{Exception}";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.WithClientIp()
    .Enrich.WithCorrelationId(headerName: "CorrelationId", addValueIfHeaderAbsence: true)
    .Enrich.WithSensitiveDataMasking(options =>
    {
        options.MaskingOperators = new List<IMaskingOperator>()
        {
            new EmailAddressMaskingOperator()
        };
    })
    .Enrich.WithHttpRequestUrl()
    .WriteTo.Console(outputTemplate: logPattern)
    .WriteTo.File(Path.Combine("logs", "trackingServices-backend-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 21,
        rollOnFileSizeLimit: true,
        outputTemplate: logPattern)
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
        DetectElasticsearchVersion = true
    }).CreateLogger();

builder.Services.AddSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo() { Title = "TrackingServicesApi", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DbExceptionHandler>();
builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");

// Add JWT Authentication
builder.Services.AddAuthentication("HttponlyAuth")
    .AddCookie("HttponlyAuth", options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "auth_token";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 5;
    });
});

builder.Services.AddCors(
    (options) =>
    {
        options.AddPolicy("AllowLocalhost", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "localhost")
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    }
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
    migrationRunner.Run();
}

app.UseMiddleware<PerformanceMiddleware>(TimeSpan.FromMilliseconds(700));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");

app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();