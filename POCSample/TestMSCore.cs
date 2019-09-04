using Microsoft.Graph;


using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using OpenCensus.Exporter.Zipkin;
using OpenCensus.Trace;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Sampler;
using System;
using System.Collections.Generic;
using System.Text;

namespace POCSample
{
    
  public class TestMSCore
    {
        public static object Run(string zipkinUri)
        {
            
        // 1. Configure exporter to export traces to Zipkin
        var exporter = new ZipkinTraceExporter(
            new ZipkinTraceExporterOptions()
            {
                Endpoint = new Uri(zipkinUri),
                ServiceName = "trace-A",
            },
            Tracing.ExportComponent);

            exporter.Start();
            

            // 2. Configure 100% sample rate for the purposes of the demo
            ITraceConfig traceConfig = Tracing.TraceConfig;
            ITraceParams currentConfig = traceConfig.ActiveTraceParams;
            var newConfig = currentConfig.ToBuilder()
                .SetSampler(Samplers.AlwaysSample)
                .Build();
            traceConfig.UpdateActiveTraceParams(newConfig);
            

            //Build application
            // Microsoft.Identity.Client
            var confidentialClientApplication = PublicClientApplicationBuilder
                .Create("efe9af1d-0c2a-499e-bc4c-1d23c22a8a87")
                .Build();

            // Microsoft.Graph.auth
            DeviceCodeProvider authenticationProvider = new DeviceCodeProvider(confidentialClientApplication);

  

            var config = new GraphTelemetryConfiguration();
            config.LatencyConfig = new LatencyConfig {
                authenticationLatencyIsEnabled = true,
                compressionHandlerLatencyIsEnabled = true,

            };
     
             var client = new GraphServiceClient(authenticationProvider: authenticationProvider, config: config);
     

            using (var scope = Tracing.Tracer.SpanBuilder("'v1.0/me' full request latency ").StartScopedSpan())
            {
                var user = client.Me.Request().GetAsync().GetAwaiter().GetResult();

                Console.WriteLine("Succesfully captured full latency of /me");
            }


            //using (var scope = Tracing.Tracer.SpanBuilder("v1.0/me/Drive/Root/Children ").StartScopedSpan())
            //{
            //    var driveItemsStream = client.Me.Drive.Items["01ZJ3LUMVJPVXOFF43LVA23PKLT3KRFL54"].Content.Request().GetAsync().GetAwaiter().GetResult();
                
            //}



            Tracing.ExportComponent.SpanExporter.Dispose();

            return null;
        }

    }
}




