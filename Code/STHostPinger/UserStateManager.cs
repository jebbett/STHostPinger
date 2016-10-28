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
                    
                        if (pingable)
                        {
                            if (!ActiveClients.Contains(element)) {
                                if (Program.DebugLevel >= 1)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(DateTime.Now.ToString("dd-MMM-yy hh:mm") + "  [ONLINE]       " + element);
                                }
                                ActiveClients.Add(element);
                                InActiveClients.Remove(element);
                                CreateEndpointUrl(element, true); 
                            }
                        }
                        else
                        {
                        
                            if (ActiveClients.Contains(element)) {
                                if (Program.DebugLevel >= 1)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine(DateTime.Now.ToString("dd-MMM-yy hh:mm") + "  [WENT OFFLINE] " + element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }
                                ActiveClients.Remove(element);
                                InActiveClients.Add(element);
                                CreateEndpointUrl(element, false);
                            }
                            else if (!InActiveClients.Contains(element))
                            {
                                if (Program.DebugLevel >= 1)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(DateTime.Now.ToString("dd-MMM-yy hh:mm") + "  [OFFLINE]      " + element);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }
                                InActiveClients.Add(element);
                                CreateEndpointUrl(element, false);
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

        private void CreateEndpointUrl(string IPAdd, bool State)
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

            SendGetRequest(endpoint);
        }


        public static string SendGetRequest(string url)
        {
            
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Program.DebugLevel >= 2) Console.WriteLine("SendGetRequest: " + url);
                    string result = client.DownloadString(url);
                    if (Program.DebugLevel == 2) Console.WriteLine("Result: " + result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (Program.DebugLevel >= 1)
                {
                    Console.WriteLine("Failed to SendGetRequest: " + ex.Message);

                }
                return null;
            }
        }

    }
}
