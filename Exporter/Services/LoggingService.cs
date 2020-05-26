using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Services
{

    public interface ILoggingService : IService
    {

    }

    public class LoggingService : Service, ILoggingService
    {

    }

}
