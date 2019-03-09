using Altered.Pipeline;
using Altered.Shared;
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
                var logType = args.Skip(2).FirstOrDefault();

                Log.Logger = new LoggerConfiguration()
                    .WithAlteredDefault()
                    .CreateLogger();

                var log = JToken.ReadFrom(new JsonTextReader(Console.In));

                switch (logType)
                {
                    default:
                        AlteredLog.Information(new
                        {
                            Name = requestName,
                            RequestId = requestId,
                            Response = log
                        });
                        break;
                    case "request":
                        AlteredLog.Information(new
                        {
                            Name = requestName,
                            RequestId = requestId,
                            Response = log
                        });
                        break;
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
