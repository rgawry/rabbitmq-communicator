using System;
using System.Configuration;

namespace Chat
{
    public class Configuration
    {
        public int Port { get { return GetPort(); } }
        public string HostName { get { return ConfigurationManager.AppSettings["host-name"]; } }

        private static int GetPort()
        {
            var portString = ConfigurationManager.AppSettings["port"];
            var portNumber = Convert.ToInt32(portString);
            return portNumber;
        }
    }
}
