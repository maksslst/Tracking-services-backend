using Application;
using Infrastructure;
using Infrastructure.Database;
using Api.ExceptionHandlers;
using Api.Middleware;
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
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DbExceptionHandler>();
builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

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

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();