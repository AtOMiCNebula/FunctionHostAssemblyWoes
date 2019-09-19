namespace FunctionHostAssemblyWoes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DICheckerFunctionFunction
    {
        public DICheckerFunctionFunction(IServiceCollection collection)
        {
            this.Collection = collection;
        }

        public IServiceCollection Collection { get; }

        [FunctionName("DIChecker")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            ILogger log)
        {
            Assembly webjobsScriptAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName == "Microsoft.Azure.WebJobs.Script, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null");
            List<string> runtimeAssemblyNames = this.ReadRuntimeAssemblies(webjobsScriptAssembly);

            IEnumerable<string> serviceTypeAssemblyNames = this.Collection.Select(d => d.ServiceType.Assembly.GetName().Name).Distinct();

            StringBuilder b = new StringBuilder("<ul>");
            foreach (string missingRuntimeAssemblyName in serviceTypeAssemblyNames.Except(runtimeAssemblyNames))
            {
                b.AppendLine($"<li>Assembly: {missingRuntimeAssemblyName}</li>");
                b.AppendLine("<ul>");
                foreach (Type type in this.Collection.Select(d => d.ServiceType).Where(st => st.Assembly.GetName().Name == missingRuntimeAssemblyName).Distinct())
                {
                    b.AppendLine($"<li>Type: {type.FullName}</li>");
                }
                b.AppendLine("</ul>");
            }
            b.AppendLine("</ul>");

            return new ContentResult
            {
                Content = b.ToString(),
                ContentType = "text/html",
            };
        }

        public List<string> ReadRuntimeAssemblies(Assembly webjobsScriptAssembly)
        {
            using (Stream stream = webjobsScriptAssembly.GetManifestResourceStream($"{webjobsScriptAssembly.GetName().Name}.runtimeassemblies.json"))
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = JsonSerializer.CreateDefault();
                JObject obj = serializer.Deserialize<JObject>(jsonReader);

                JArray arr = obj["runtimeAssemblies"] as JArray;
                return arr.Select(o => o["name"].ToString()).ToList();
            }
        }
    }
}
