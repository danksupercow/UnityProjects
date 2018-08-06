using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

public class ServerTCP
{
    //Server Socket
    public static TcpListener serverSocket;

    //Server Client Info
    public static Client[] Clients = new Client[0];
    public static Players SavedPlayers = new Players();

    //Server Item Info
    public static string[] itemFiles;

    public static void InitNetwork()
    {
        serverSocket = new TcpListener(IPAddress.Any, 5555);
        serverSocket.Start();
        serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        LoadJsonItems();
        Clients = new Client[Constants.GAMERULES.MAX_PLAYER_COUNT];
        General.LoadPlayersInfo();
        Console.WriteLine(SavedPlayers.Count);
        Console.WriteLine("Server Started Successfully :D");
    }
    public static void CloseNetwork()
    {
        serverSocket.Stop();
    }

    private static void OnClientConnect(IAsyncResult ar)
    {
        TcpClient client = serverSocket.EndAcceptTcpClient(ar);
        serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        client.NoDelay = false;

        for (int i = 0; i < Constants.GAMERULES.MAX_PLAYER_COUNT; i++)
        {
            if (Clients[i].socket == null)
            {
                Clients[i] = new Client(client, i, client.Client.RemoteEndPoint.ToString());
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Connection received from " + Clients[i].ip);
                Console.ResetColor();
                SendWelcomeMessage(i);
                for (int x = 0; x < itemFiles.Length; x++)
                {
                    SendItemDataTo(i, itemFiles[x]);
                }
                SendInWorld(i);
                return;
            }
        }

        Console.WriteLine("Failed To Find Empty Slot For Player!");
    }

    public static byte[] PlayerData(int connectionID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerData);
        buffer.WriteInteger(connectionID);

        Player p = Clients[connectionID].player;
        buffer.WriteFloat(p.PosX);
        buffer.WriteFloat(p.PosY);
        buffer.WriteFloat(p.PosZ);

        buffer.WriteFloat(p.Health);
        buffer.WriteFloat(p.Hunger);
        buffer.WriteFloat(p.Thirst);

        return buffer.ToArray();
    }

    public static void SendDataTo(long connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
        buffer.WriteBytes(data);
        Clients[connectionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
        buffer = null;
    }

    private static void SendDataToAll(byte[] data)
    {
        for (int i = 0; i < Constants.GAMERULES.MAX_PLAYER_COUNT; i++)
        {
            if (Clients[i].socket != null)
            {
                SendDataTo(i, data);
            }
        }
    }

    public static void SendWelcomeMessage(int connectionID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerJoined);
        buffer.WriteInteger(connectionID);
        buffer.WriteString("[Server] Welcome To The Server!");

        SendDataTo(connectionID, buffer.ToArray());
        buffer.Dispose();

        SendGameRules(connectionID);
    }

    public static void SendInWorld(int connectionID)
    {
        //Send all players on current scene to the player itself
        for (int i = 0; i < Constants.GAMERULES.MAX_PLAYER_COUNT; i++)
        {
            if (Clients[i].socket != null)
            {
                if (i != connectionID)
                {
                    SendDataTo(connectionID, PlayerData(i));
                }
            }
        }
        //sends connectionID player data to everyone on server including self
        SendDataToAll(PlayerData(connectionID));
    }

    public static void SendPlayerLeft(int connectionID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerLeft);
        buffer.WriteInteger(connectionID);

        SendDataToAll(buffer.ToArray());

        buffer.Dispose();
    }

    public static void SendItemDataTo(int connectionID, string itemFile)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.ItemData);
        buffer.WriteString(itemFile);

        SendDataTo(connectionID, buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendPlayerMove(int connectionID, float x, float y, float z, float rotX, float rotY, float rotZ)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerMove);
        buffer.WriteInteger(connectionID);
        buffer.WriteFloat(x);
        buffer.WriteFloat(y);
        buffer.WriteFloat(z);

        buffer.WriteFloat(rotX);
        buffer.WriteFloat(rotY);
        buffer.WriteFloat(rotZ);

        SendDataToAll(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendPlayerStats(int connectionID, float health, float hunger, float thirst)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerStats);
        buffer.WriteInteger(connectionID);

        buffer.WriteFloat(health);
        buffer.WriteFloat(hunger);
        buffer.WriteFloat(thirst);

        SendDataToAll(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendDamage(int connectionID, float damage)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.Damage);
        buffer.WriteInteger(connectionID);
        buffer.WriteFloat(damage);

        SendDataToAll(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendGameRules(int connectionID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.GameRules);
        buffer.WriteFloat(Constants.GAMERULES.MAX_PLAYER_HEALTH);
        buffer.WriteFloat(Constants.GAMERULES.PLAYER_STARTING_HEALTH);

        SendDataTo(connectionID, buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendNetSpawnRequest(int connectionID, string slug, float x, float y, float z, float rotX, float rotY, float rotZ)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.NetSpawn);
        buffer.WriteInteger(connectionID);
        buffer.WriteString(slug);
        buffer.WriteFloat(x);
        buffer.WriteFloat(y);
        buffer.WriteFloat(z);

        buffer.WriteFloat(rotX);
        buffer.WriteFloat(rotY);
        buffer.WriteFloat(rotZ);

        SendDataToAll(buffer.ToArray());
        buffer.Dispose();
    }

    private static void LoadJsonItems()
    {
        try
        {
            itemFiles = General.JsonFiles(Constants.ITEMSPATH);
            Console.WriteLine("Items Loaded Successfully.");
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ResetColor();
        }
    }
    public static void LoadJsonGameRules()
    {
        Constants.GAMERULES = General.FromJson();
        if (Constants.GAMERULES != null)
        {
            Console.WriteLine(Constants.GAMERULES.ToString());
        }
    }
    

    /*
    public static void SendJoinMap(long index)
    {
        for (int i = 0; i < Constants.MAX_PLAYERS; i++)
        {
            if(Clients[i].socket != null)
            {
                if(i != index)
                {
                    ByteBuffer buffer = new ByteBuffer();
                    buffer.WriteLong((long)PacketType.PlayerJoined);
                    buffer.WriteInteger(i);
                    SendDataTo(index, buffer.ToArray());

                    ByteBuffer buffer2 = new ByteBuffer();
                    buffer2.WriteLong((long)ServerPackets.SJoinMap);
                    buffer2.WriteInteger((int)index);
                    SendDataTo(index, buffer2.ToArray());
                }
            }
        }
    }
    */

}
