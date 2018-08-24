using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        private static Thread consoleThread;

        private static void Main(string[] args)
        {
            consoleThread = new Thread(ConsoleThread);
            consoleThread.Start();
            InitServer();
        }

        private static void InitServer()
        {
            TCPServer.Init();
        }

        private static void ConsoleThread()
        {
            while(true)
            {
                Console.ReadLine();
            }
        }
    }
}

public static class Debug
{
    private const ConsoleColor errorColor = ConsoleColor.DarkRed;
    private const ConsoleColor warningColor = ConsoleColor.DarkYellow;
    private const ConsoleColor updateColor = ConsoleColor.DarkCyan;

    public static void Log(object input)
    {
        Console.WriteLine(input);
    }

    public static void Log(object input, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(input);
        Console.ResetColor();
    }

    public static void LogError(object input)
    {
        Log("[ERROR] " + input, errorColor);
    }

    public static void LogWarning(object input)
    {
        Log("[WARNING] " + input, warningColor);
    }

    public static void LogUpdate(object input)
    {
        Log("[UPDATE] " + input, updateColor);
    }
}
