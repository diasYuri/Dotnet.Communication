using Dotnet.Communication.gRPC.Server;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using StatusCode = Grpc.Core.StatusCode;

namespace Dotnet.Communication.gRPC.Client.Extensions;

public static class DIExtensions
{
    public static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }
    
    public static IServiceCollection AddgRpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddGrpcClient<Auth.AuthClient>(o =>
            {
                o.Address = new Uri("https://localhost:7172");
            }).ConfigureChannel(options =>
            {
                var defaultMethodConfig = new MethodConfig
                {
                    Names = { MethodName.Default },
                    RetryPolicy = new RetryPolicy
                    {
                        MaxAttempts = 5,
                        InitialBackoff = TimeSpan.FromSeconds(1),
                        MaxBackoff = TimeSpan.FromSeconds(5),
                        BackoffMultiplier = 1.5,
                        RetryableStatusCodes = { StatusCode.Unavailable, StatusCode.Internal, StatusCode.Aborted }
                    }
                };

                options.ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } };
            });

        return services;
    }

    public static  WebApplicationBuilder AddOpenTelemetry(this  WebApplicationBuilder builder, string applicationName, IConfiguration configuration)
    {
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

        return builder;
    }
}