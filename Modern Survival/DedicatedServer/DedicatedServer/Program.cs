using System;
using System.Windows;
using System.Threading;

namespace DedicatedServer
{
    class Program
    {
        private static Thread threadConsole;
        private static string command;

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
                command = Console.ReadLine().ToLower();

                if(command == "quit" || command == "stop")
                {
                    ServerTCP.CloseNetwork();
                    Environment.Exit(0);
                }

                if(command == "newprop")
                {
                    General.CreateGameRulesFile();
                }

                if(command.StartsWith("kick"))
                {
                    string s = string.Empty;
                    if(command.Length >= 7)
                        s = command.Substring(7);
                    string[] temps = command.Split(' ');
                    ServerTCP.Clients[int.Parse(temps[1])].Kick(s);
                }
            }
        }
    }
}
