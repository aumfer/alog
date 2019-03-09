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
            // todo build an AlteredPipeline that wraps abitrary Func<TRequest, Task<TResponse>>
            // and runs it in a seperate process by serializing its arguments as json via stdin/out/err
            //
            // then do the same for EOF-seperated json into observable streams
            try
            {
                var requestName = args.Skip(0).FirstOrDefault();
                var requestId = args.Skip(1).FirstOrDefault();

                Log.Logger = new LoggerConfiguration()
                    .WithAlteredDefault()
                    .Destructure.JsonNetTypes()
                    .CreateLogger();

                var logData = JToken.ReadFrom(new JsonTextReader(Console.In));
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
