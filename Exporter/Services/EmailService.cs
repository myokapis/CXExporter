using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Services
{

    public interface IEmailService : IService
    {
        void SendNotifications();
    }

    public class EmailService : Service, IEmailService
    {

        public void SendNotifications()
        {
            // TODO: implement this
        }

    }

}
