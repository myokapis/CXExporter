//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.IO;
//using System.Text;
//using Microsoft.Extensions.Configuration;
//using Exporter.Extensions;
//using Exporter.Models;

//namespace Exporter.Services
//{

//    //public interface IConfigurationService
//    //{
//    //    Process GetProcessConfig(string orgCode, string processName);
//    //}

//    // TODO: support retrieval from both db and json files
//    // TODO: move this to the main service application
//    /// <summary>
//    /// Reads configuration files and database configuration
//    /// </summary>
//    public static class ConfigurationService //: IConfigurationService
//    {
//        //private readonly DirectoryInfo exportRoot;
//        //private readonly string orgCode;
//        //private readonly string processName;

//        //public ConfigurationService(DirectoryInfo exportRoot, string orgCode, string processName)
//        //{
//        //    this.exportRoot = exportRoot;
//        //    this.orgCode = orgCode;
//        //    this.processName = processName;
//        //}

//        //public static DirectoryInfo ExportBaseDirectory(string orgCode)
//        //{
//        //    var path = Path.Combine(AppConfig["ExportBaseDirectory"], orgCode);
//        //    return new DirectoryInfo(path);
//        //}

//        // TODO: this will eventually all come from the database
//        public static Process GetProcessConfig(DirectoryInfo orgRoot, string processName)
//        {
//            // get the path to the org's process
//            var fileName = $"ExportConfig_{processName}.json";

//            // get the config from the file
//            var config = new ConfigurationBuilder()
//                .SetBasePath(orgRoot.DirectoryIn("ExportConfigs").FullName)
//                .AddJsonFile(fileName, true)
//                .Build();

//            return config.GetSection("Process").Get<Process>();
//        }

//    }

//}
