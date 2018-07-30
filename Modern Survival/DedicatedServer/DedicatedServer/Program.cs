using System;
using System.Windows;
using System.Threading;

namespace DedicatedServer
{
    class Program
    {
        private static Thread threadConsole;
        static void Main(string[] args)
        {
            threadConsole = new Thread(new ThreadStart(ConsoleThread));
            threadConsole.Start();
            SetupServer();
            //starting server

        }

        static void SetupServer()
        {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                ServerTCP.Clients[i] = new Client();
                Types.TempPlayer[i] = new Types.TempPlayerRec();
            }

            ServerHandleData.Init();
            ServerTCP.InitNetwork();
        }

        static void ConsoleThread()
        {
            while(true)
            {
                if(Console.ReadLine().ToLower() == "quit" || Console.ReadLine().ToLower() == "stop")
                {
                    ServerTCP.CloseNetwork();
                    Environment.Exit(0);
                }
                Console.ReadLine();
            }
        }

        public static void JoinGame(int connectionID)
        {
            ServerTCP.SendInWorld(connectionID);
        }
    }
}
