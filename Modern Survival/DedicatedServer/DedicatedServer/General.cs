﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

public class General
{
    public static byte[] FromString(string s)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        try
        {
            bf.Serialize(ms, s);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        ms.Close();
        return ms.ToArray();
    }
    public static string FromBinaryBytes(byte[] bytes)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();

        try
        {
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        ms.Close();
        return bf.Deserialize(ms).ToString();
    }
    public static string[] JsonFiles(string path)
    {
        string[] dirs = Directory.GetFiles(path, "*.json");
        List<string> jsonFiles = new List<string>();
        for (int i = 0; i < dirs.Length; i++)
        {
            jsonFiles.Add(File.ReadAllText(dirs[i]));
        }

        return jsonFiles.ToArray();
    }
    private static string FetchGameRulesJson()
    {
        if(File.Exists(Constants.GAMERULESPATH))
        {
            return File.ReadAllText(Constants.GAMERULESPATH);
        }
        else
        {
            return "ERROR Failed To Fetch GameRules.json File, Does One Exist?";
        }
    }
    public static Properties FromJson()
    {
        string json = FetchGameRulesJson();

        if(json.StartsWith("ERROR"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(json);
            Console.ResetColor();
            return null;
        }
        return JsonConvert.DeserializeObject<Properties>(json);
    }
    public static Properties CreateGameRulesFile()
    {
        if(File.Exists(Constants.GAMERULESPATH))
        {
            return FromJson();
        }

        File.Create(Constants.GAMERULESPATH);
        File.WriteAllText("D:/", new Properties().ToJson());
        return FromJson();
    }

    public static void WritePlayersInfo()
    {
        string json = JsonConvert.SerializeObject(ServerTCP.Players, Formatting.Indented);
        if (File.Exists(Constants.PLAYERSINFOPATH))
        {
            File.WriteAllText(Constants.PLAYERSINFOPATH, json);
            return;
        }
        
        File.WriteAllText(Constants.PLAYERSINFOPATH, json);
    }

    public static void LoadPlayersInfo()
    {
        string json = File.ReadAllText(Constants.PLAYERSINFOPATH);
        ServerTCP.Players = JsonConvert.DeserializeObject(json) as List<Player>;
        Console.WriteLine(json);
    }
}