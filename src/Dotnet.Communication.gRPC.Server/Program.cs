using Dotnet.Communication.gRPC.Server.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var applicationName = builder.Environment.ApplicationName;
builder.Services.AddOpenTelemetryMetrics(builderMetrics =>
{
    builderMetrics.AddHttpClientInstrumentation();
    builderMetrics.AddAspNetCoreInstrumentation();
    builderMetrics.AddMeter(applicationName);
    builderMetrics.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});
        
builder.Services.AddOpenTelemetryTracing(builderTracing => 
{
    builderTracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName))
        .AddSource(applicationName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddOtlpExporter(opts =>  opts.Endpoint = new Uri("http://localhost:4317"));
});
        
builder.Logging.AddOpenTelemetry(builderLogger =>
{
    builderLogger.IncludeFormattedMessage = true;
    builderLogger.IncludeScopes = true;
    builderLogger.ParseStateValues = true;
    builderLogger.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<AuthService>();

app.Run();