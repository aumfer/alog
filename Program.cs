using Altered.Aws;
using Altered.Aws.Cloudwatch;
using Altered.Pipeline;
using Altered.Shared;
using Destructurama;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace alog
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var requestName = args.Skip(0).FirstOrDefault();
                var requestId = args.Skip(1).FirstOrDefault();

                var services = new ServiceCollection()
                    .EnsureAwsRegion()
                    .AddAlteredAws()
                    .AddCloudwatchLogs()
                    .BuildServiceProvider();

                var logSink = services.GetService<CloudwatchLogsSink>();

                using (var reader = new JsonTextReader(Console.In)
                {
                    SupportMultipleContent = true
                })
                {
                    while (reader.Read())
                    {
                        var logData = JToken.ReadFrom(reader);
                        var log = new
                        {
                            Name = requestName,
                            RequestId = requestId,
                            Message = logData
                        };

                        var logJson = logSink.Log(log);
                        Console.WriteLine($"{logJson}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"{e}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
