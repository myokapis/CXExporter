using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Exporter.Extensions;
using Exporter.Models;
using Exporter.Services;

namespace DNCSandbox
{

    // TODO: write this to behave the way the windows service will behave
    //       except this will run just once for demo purposes rather than be a long running process
    internal class Program
    {

        // TODO: need a place to parse options to get orgCode, processName, batchId
        //       actually when this is a service, those values will be passed in by the caller
        //       probably need to make this a service so that it can be called from a scheduler or a UI
        //       or use the db as the link to all services; queue the requests there and let the service pick them up
        // TODO: eventually make all of this async

        static void Main(string[] args)
        {
            // defaulting values that will be passed in by the main service
            // TODO: decide if the config service should live in the main service
            //       and pass in the actual config instead of the values listed below
            var orgCode = "LionKing";
            var processCode = "PrideRock";
            var batchId = 1;

            var exportRoot = new DirectoryInfo(AppConfig["ExportBaseDirectory"]);
            var config = GetConfig(exportRoot, orgCode, processCode);
            var services = ServicesFactory.GetServices(exportRoot, config.OrgUnit, config.Process);
            var exportService = new ExportService(services);
            exportService.Run(batchId);
        }

        private static IConfigurationRoot AppConfig => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("AppConfig.json", false, true)
            .Build();

        // TODO: add custom converter for converting string to type - OR - switch to JSONConvert
        private static Config GetConfig(DirectoryInfo exportRoot, string orgCode, string processCode)
        {
            // get the config file by convention
            var fileName = $"ExportConfig_{processCode}.json";

            // setup up a json configuration reader
            var config = new ConfigurationBuilder()
                .SetBasePath(exportRoot.DirectoryIn(orgCode, "ExportConfigs").FullName)
                .AddJsonFile(fileName, true)
                .Build();

            // read the config from the file
            return config.GetSection("Config").Get<Config>();
        }

        private class Config
        {
            public OrgUnit OrgUnit { get; set; }
            public Process Process { get; set; }
        }

    }

}
