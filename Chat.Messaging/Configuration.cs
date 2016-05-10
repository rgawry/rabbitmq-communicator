using System;
using System.Configuration;

namespace Chat
{
    public class Configuration
    {
        public int Port { get { return GetPort(); } }
        public string HostName { get { return ConfigurationManager.AppSettings["host-name"]; } }
        public string ExchangeRequestName { get { return ConfigurationManager.AppSettings["exchange-request-name"]; }  }
        public string QueueRequestName { get { return ConfigurationManager.AppSettings["queue-request-name"]; }  }

        private static int GetPort()
        {
            var portString = ConfigurationManager.AppSettings["port"];
            var portNumber = Convert.ToInt32(portString);
            return portNumber;
        }
    }
}
