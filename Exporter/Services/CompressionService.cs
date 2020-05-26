using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Services
{

    public interface ICompressionService : IService
    {
        void CompressFiles();
    }

    public class CompressionService : Service, ICompressionService
    {

        public void CompressFiles()
        {

        }

    }

}
