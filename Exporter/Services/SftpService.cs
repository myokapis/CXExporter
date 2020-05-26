using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Services
{

    public interface ISftpService : IService
    {
        void SendFiles();
    }

    public class SftpService : Service, ISftpService
    {

        public void SendFiles()
        {
            // TODO: implement
        }

    }

}
