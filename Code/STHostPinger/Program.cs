using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace jebbett
{
    class Program
    {
        public static int DebugLevel = 1;
        private static UserStateManager StateManager;

        static void Main()
        {
            Console.Title = "SmartThings Host Pinger";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---SmartThings Host Pinger V2.0---");
            Console.WriteLine(" > Loading config..");
            if (!Config.TryLoad()) return;
            Console.WriteLine(" > Loaded");

            //Set debug level
            DebugLevel = Config.ConsoleDebugLevel;
            Console.WriteLine(" > DebugLevel is set to " + DebugLevel);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" > Running ...... ");
            StateManager = new UserStateManager();

            /////////////////// NEW

            //Thread thread = new Thread(() => CheckIP());
            CheckIP();

            //thread.Start();
            Console.ReadLine();
            Terminate = true;
            //thread.Abort();

            Console.ForegroundColor = ConsoleColor.Red;
            
            Console.Write("\n\nTerminating Process..");
            
            Thread.Sleep(500);
            System.Environment.Exit(0);
        }

        public static bool Terminate = false;

        private static void CheckIP()
        {
            while (!Terminate)
            {
                if (DebugLevel >= 2) Console.WriteLine(" > Checking..");
                WebClient client = new WebClient();
                StateManager.ParseIPs();
                if (Terminate) return;

                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(Config.CheckInterval));
                if (Terminate) return;
            }
        }

        
        
        
    }
}
