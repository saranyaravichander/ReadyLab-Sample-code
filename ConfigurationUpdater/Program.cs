using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;


namespace ConfigurationUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            string uniqueValue = ConfigurationSettings.AppSettings["aliasOrUniquevalue"];

            // here, you can now decide what to do - for demo purposes,
            // I just set the ID value to a fixed value if it was empty before
            if (!string.IsNullOrEmpty(uniqueValue))
            {
                string json = File.ReadAllText(@"..\..\..\ProvisioningProject\azuredeploy.parameters.json");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["parameters"]["hubName"]["value"] = "iothub-" + uniqueValue;
                jsonObj["parameters"]["storageName"]["value"] = "storage" + uniqueValue;
                jsonObj["parameters"]["appServiceName"]["value"] = "function-" + uniqueValue;
                jsonObj["parameters"]["appServicePlanName"]["value"] = "appserviceplan-" + uniqueValue;
                jsonObj["parameters"]["resourceGroupName"]["value"] = "labrg-" + uniqueValue;
                jsonObj["parameters"]["apphostplan"]["value"] = "apphostplan-" + uniqueValue;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(@"..\..\..\ProvisioningProject\azuredeploy.parameters.json", output);
            }
        }
    }
}
