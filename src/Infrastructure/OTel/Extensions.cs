using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infrastructure.OTel;

public static class Extensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName);
        var otlpEndpoint = builder.Configuration.GetValue<string>("Otlp:Endpoint");

        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            builder.Logging.AddOpenTelemetry(logging => logging.SetResourceBuilder(resourceBuilder)
                // .AddOtlpExporter());
            .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otlpEndpoint);
                    otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                }));
        }

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                // metrics.SetResourceBuilder(resourceBuilder)
                //     .AddPrometheusExporter()
                //     .AddAspNetCoreInstrumentation()
                //     .AddRuntimeInstrumentation()
                //     .AddHttpClientInstrumentation()
                //     // .AddEventCountersInstrumentation(c =>
                //     // {
                //     //     // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                //     //     c.AddEventSources(
                //     //         "Microsoft.AspNetCore.Hosting",
                //     //         "Microsoft-AspNetCore-Server-Kestrel",
                //     //         "System.Net.Http",
                //     //         "System.Net.Sockets",
                //     //         "System.Net.NameResolution",
                //     //         "System.Net.Security");
                //     // }
                //     .AddMeter("Microsoft.AspNetCore.Hosting")
                //     .AddMeter("Microsoft-AspNetCore-Server-Kestrel")
                //     .AddMeter("System.Net.Http");
                //     // .AddOtlpExporter();
                // if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                // {
                //     metrics.AddOtlpExporter(otlpOptions =>
                //     {
                //         otlpOptions.Endpoint = new Uri(otlpEndpoint);
                //         otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                //     });
                // }
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                metrics.AddPrometheusExporter();
            })
            .WithTracing(tracing =>
            {
                // We need to use AlwaysSampler to record spans
                // from Todo.Web.Server, because there it no OpenTelemetry
                // instrumentation
                tracing.SetResourceBuilder(resourceBuilder)
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });

        return builder;
    }
}