using System;
using System.Collections.Generic;
using System.Web;
using System.Net.NetworkInformation;
using System.Net;

namespace jebbett
{
    class UserStateManager
    {
        public List<String> ActiveClients = new List<String>();
        public List<String> InActiveClients = new List<String>();
        

        public void ParseIPs()
        {            
            //Loop through results
            try {
                foreach (string element in Config.HostList)
                {
                    using ( Ping pinger = new Ping()) { 
                        PingReply reply = pinger.Send(element, Config.TimeOut);
                        bool pingable = reply.Status == IPStatus.Success;
                        
                    
                        if (pingable){
                            if (!ActiveClients.Contains(element) || Config.HeartBeat) {
                                if (Program.DebugLevel >= 1){
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(DateTime.Now.ToString("dd-MMM-yy HH:mm") + "  [ONLINE]       " + element);
                                }
                                if(CreateEndpointUrl(element, true)){
                                    ActiveClients.Add(element);
                                    InActiveClients.Remove(element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Successfully sent to App");
                                }
                            }
                        }
                        else
                        {
                            if (ActiveClients.Contains(element)) {
                                if (Program.DebugLevel >= 1){
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine(DateTime.Now.ToString("dd-MMM-yy HH:mm") + "  [WENT OFFLINE] " + element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }
                                if (CreateEndpointUrl(element, false))
                                {
                                    ActiveClients.Remove(element);
                                    InActiveClients.Add(element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Successfully sent to App");
                                }
                            }
                            else if (!InActiveClients.Contains(element) || Config.HeartBeat)
                            {
                                if (Program.DebugLevel >= 1){
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(DateTime.Now.ToString("dd-MMM-yy HH:mm") + "  [OFFLINE]      " + element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }
                                if (CreateEndpointUrl(element, false)){
                                    InActiveClients.Add(element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Successfully sent to App");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Uh Oh: " + ex.Message);
                return;
            }
            return;
        }

        public static bool CreateEndpointUrl(string IPAdd, bool State)
        {
            string endpoint = "";
            if (State == true) endpoint = Config.EndpointUrl_Online;
            else if (State == false) endpoint = Config.EndpointUrl_Offline;

            //Prepare for first param, just in case there is already added some params in the url in the config file
            if (!endpoint.Contains("?")) endpoint += "?";
            else endpoint += "&";

            //Add the GET parameters to the request
            endpoint += "access_token=" + HttpUtility.UrlEncode(Config.Endpoint_AccessToken);
            endpoint += "&ipadd=" + HttpUtility.UrlEncode(IPAdd);
            return SendGetRequest(endpoint);

        }


        public static bool SendGetRequest(string url)
        {
            
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Program.DebugLevel >= 2) Console.WriteLine("SendGetRequest: " + url);
                    string result = client.DownloadString(url);
                    if (Program.DebugLevel == 2) Console.WriteLine("Result: " + result);
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (Program.DebugLevel >= 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to send request to App: " + ex.Message + " [RETRYING..]");
                }
                return false;
            }
        }

    }
}
