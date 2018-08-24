using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Command
{
    private static string command;

    public static void Check()
    {
        command = Console.ReadLine().ToLower();

        if (command == "quit" || command == "stop")
        {
            ServerTCP.CloseNetwork();
            Environment.Exit(0);
        }

        if (command == "newprop")
        {
            General.CreateGameRulesFile();
        }

        if (command.StartsWith("kick"))
        {
            string s = string.Empty;
            if (command.Length >= 7)
                s = command.Substring(7);
            string[] temps = command.Split(' ');
            ServerTCP.Clients[int.Parse(temps[1])].Kick(s);
        }

        if(command.StartsWith("clear"))
        {
            Console.Clear();
        }
    }
}
