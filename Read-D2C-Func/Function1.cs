using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;


using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;
using System.IO;

namespace Read_D2C_Func
{
    public static class IotHubTrigger
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("IotHubTrigger")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IOTHUBCONNECTION")]EventData message,
            [Blob("iothuboutput/{rand-guid}", FileAccess.Write, Connection = "OUTBLOBSTORAGE")] Stream iotOut,
            ILogger log, 
            ExecutionContext context)
        {
            var iotHubMsg = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation($"C# IoT Hub trigger function processed a message: {iotHubMsg}");

            iotOut = new MemoryStream(Encoding.UTF8.GetBytes(iotHubMsg ?? ""));
        }
    }
}