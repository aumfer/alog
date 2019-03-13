using Altered.Pipeline;
using Altered.Shared;
using Destructurama;
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

                Log.Logger = new LoggerConfiguration()
                    .WithAlteredDefault()
                    .Destructure.JsonNetTypes()
                    .CreateLogger();

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

                        AlteredLog.Information(log);

                        var logJson = JsonConvert.SerializeObject(log, AlteredJson.DefaultJsonSerializerSettings);
                        Console.WriteLine(logJson);
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
