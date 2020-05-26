using System;
using System.Collections.Generic;
using System.Text;
using Exporter.Models;

namespace Exporter.Services
{

    // TODO: decide if a factory is required or if the service can be constructed 
    public static class ExportServiceFactory
    {

        public static IExportService GetExportService(IServices serviceProvider)
        {
            return new ExportService(serviceProvider);
        }

    }

}
