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
            Console.WriteLine("Starting Server...");

            ServerTCP.LoadJsonGameRules();
            Types.TempPlayer = new Types.TempPlayerRec[Constants.GAMERULES.MAX_PLAYER_COUNT];

            ServerTCP.InitNetwork();
            ServerHandleData.Init();

            for (int i = 0; i < Constants.GAMERULES.MAX_PLAYER_COUNT; i++)
            {
                ServerTCP.Clients[i] = new Client();
                Types.TempPlayer[i] = new Types.TempPlayerRec();
            }
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
    }
}
