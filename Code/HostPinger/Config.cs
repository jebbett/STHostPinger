using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace jebbett
{
    static class Config
    {
        public static int CheckInterval { get; private set; }
        public static int TimeOut { get; private set; }
		public static int ConsoleDebugLevel { get; private set; }
        public static bool HeartBeat { get; private set; }
        public static string Endpoint_AccessToken { get; private set; }
        public static string EndpointUrl_Online { get; private set; }
        public static string EndpointUrl_Offline { get; private set; }

        //public static List<string> WhiteList { get; private set; }
        public static List<string> HostList = new List<string>() { };

        public static bool TryLoad()
        {
            try
            {
                Load();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error in config: "+ex.Message);
                Console.ReadLine();
                return false;
            }
        }

        
        private static void Load()
        {
            XDocument doc = null;
            doc = XDocument.Load("config.config");
            
            XElement eConfig = doc.XPathSelectElement("/config");
            
            //Settings
            XElement IPCheck = eConfig.Element("pingSettings");
            CheckInterval = int.Parse(IPCheck.Attribute("checkInterval").Value);
            TimeOut = int.Parse(IPCheck.Attribute("timeOut").Value);
            ConsoleDebugLevel = int.Parse(IPCheck.Attribute("debugLevel").Value);
            HeartBeat = bool.Parse(IPCheck.Attribute("heartBeat").Value);

            //App Endpoints

            XElement eSTE = eConfig.Element("theEndpoints");
            Endpoint_AccessToken = eSTE.Attribute("accessToken").Value;
            EndpointUrl_Online = eSTE.Attribute("Online").Value;
            EndpointUrl_Offline = eSTE.Attribute("Offline").Value;

            // Process White List
            
            XElement eHostList = eConfig.Element("hostList");

            foreach (XElement item in eHostList.Elements())
            {
                //////////////////////// Check for IP match using variable:    item.Attribute("value").Value
                HostList.Add(item.Attribute("HOST").Value);
            }
        }
    }
}
