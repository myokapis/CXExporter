using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Services
{

    public interface IEncryptionService : IService
    {
        void EncryptFiles();
    }

    public class EncryptionService : Service, IEncryptionService
    {

        public void EncryptFiles()
        {
            // TODO: implement
        }

    }

}
