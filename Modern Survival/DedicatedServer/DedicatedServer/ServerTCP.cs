using System;
using System.Collections;
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

    public struct ServerData
    {
        public List<SpawnedPrefab> spawnedPrefabs;
        public int Index { get { return spawnedPrefabs.Count; } }

        public void RegisterObject(string slug, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW)
        {
            SpawnedPrefab sp;
            Vector3 pos;
            Quaternion rot;
            Transform t;

            sp.slug = slug;
            sp.assignedID = Index + 1;

            pos.x = x;
            pos.y = y;
            pos.z = z;

            rot.x = rotX;
            rot.y = rotY;
            rot.z = rotZ;
            rot.w = rotW;

            t.position = pos;
            t.rotation = rot;

            sp.transform = t;

            spawnedPrefabs.Add(sp);
        }
        public void UpdateObject(int objectID, Vector3 pos)
        {
            int index = objectID - 1;
            if (index == -1 || index > spawnedPrefabs.Count - 1)
                return;
            SpawnedPrefab sp = spawnedPrefabs[index].Copy();
            sp.transform.position = pos;
            spawnedPrefabs[index] = sp;
        }
        public void UpdateObject(int objectID, Quaternion rot)
        {
            int index = objectID - 1;
            if (index == -1 || index > spawnedPrefabs.Count - 1)
                return;

            SpawnedPrefab sp = spawnedPrefabs[index].Copy();
            sp.transform.rotation = rot;
            spawnedPrefabs[index] = sp;
        }
    }

    private static ServerData _data;

    public static void InitNetwork()
    {
        serverSocket = new TcpListener(IPAddress.Any, 5555);
        serverSocket.Start();
        serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        Clients = new Client[Constants.GAMERULES.MAX_PLAYER_COUNT];

        _data.spawnedPrefabs = new List<SpawnedPrefab>();   //                    <----[ This is where ill add loading spawned objects on startup from saved file later ]

        //LoadJsonItems();                                                        <----[  ADD BACK LATER  ]
        //General.LoadPlayersInfo();                                              <----[  ADD BACK LATER  ]


        Console.WriteLine("Server Started Successfully :D");
    }
    public static void CloseNetwork()
    {
        serverSocket.Stop();
    }

    private static void OnClientConnect(IAsyncResult ar)
    {
        if (serverSocket == null || ar == null)
        {
            Console.WriteLine("Result Returned NULL");
            return;
        }

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

                /*
                for (int x = 0; x < itemFiles.Length; x++)
                {
                    SendItemDataTo(i, itemFiles[x]);
                }*/

                SendInWorld(i);
                SendSpawnedObjects(i);
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
        buffer.WriteFloat(p.Transform.position.x);
        buffer.WriteFloat(p.Transform.position.y);
        buffer.WriteFloat(p.Transform.position.z);

        buffer.WriteFloat(p.Transform.rotation.x);
        buffer.WriteFloat(p.Transform.rotation.y);
        buffer.WriteFloat(p.Transform.rotation.z);
        buffer.WriteFloat(p.Transform.rotation.w);

        buffer.WriteFloat(p.Health);
        buffer.WriteFloat(p.Hunger);
        buffer.WriteFloat(p.Thirst);

        return buffer.ToArray();
    }

    public static void SendDataTo(long connectionID, byte[] data)
    {
        try
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            Clients[connectionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer = null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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

    private static void SendDataToAllBut(int connectionId, byte[] data)
    {
        for (int i = 0; i < Constants.GAMERULES.MAX_PLAYER_COUNT; i++)
        {
            if (connectionId != i)
                if (Clients[i].socket != null)
                    SendDataTo(i, data);
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
        Console.WriteLine("Sent all Players in world to: " + connectionID);
    }

    public static void SendSpawnedObjects(int connectionID)
    {
        ByteBuffer buffer = new ByteBuffer();

        for (int i = 0; i < _data.Index; i++)
        {
            SpawnedPrefab sp = _data.spawnedPrefabs[i];
            buffer.WriteLong((long)PacketType.NetSpawn);
            
            buffer.WriteInteger(sp.assignedID);

            buffer.WriteString(sp.slug);

            buffer.WriteFloat(sp.transform.position.x);
            buffer.WriteFloat(sp.transform.position.y);
            buffer.WriteFloat(sp.transform.position.z);

            buffer.WriteFloat(sp.transform.rotation.x);
            buffer.WriteFloat(sp.transform.rotation.y);
            buffer.WriteFloat(sp.transform.rotation.z);
            buffer.WriteFloat(sp.transform.rotation.w);

            SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
    }

    public static void SendPlayerLeft(int connectionID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerLeft);
        buffer.WriteInteger(connectionID);
        buffer.WriteString(string.Empty);

        SendDataToAll(buffer.ToArray());

        buffer.Dispose();
    }

    public static void SendPlayerLeft(int connectionID, string reason)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerLeft);
        buffer.WriteInteger(connectionID);
        buffer.WriteString(reason);

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

    public static void SendNetSpawnRequest(int connectionID, string slug, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW)
    {
        _data.RegisterObject(slug, x, y, z, rotX, rotY, rotZ, rotW);

        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.NetSpawn);
        
        buffer.WriteInteger(_data.Index);

        buffer.WriteString(slug);

        buffer.WriteFloat(x);
        buffer.WriteFloat(y);
        buffer.WriteFloat(z);

        buffer.WriteFloat(rotX);
        buffer.WriteFloat(rotY);
        buffer.WriteFloat(rotZ);
        buffer.WriteFloat(rotW);

        Console.WriteLine(_data.Index);

        SendDataToAll(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendSyncPosition(int connectionID, long type, int syncObjID, float x, float y, float z)
    {
        ByteBuffer buffer = new ByteBuffer();

        buffer.WriteLong((long)PacketType.SyncPosition);
        buffer.WriteLong(type);
        buffer.WriteInteger(syncObjID);

        buffer.WriteFloat(x);
        buffer.WriteFloat(y);
        buffer.WriteFloat(z);

        _data.UpdateObject(syncObjID, new Vector3(x, y, z));

        SendDataToAllBut(connectionID, buffer.ToArray());
        buffer.Dispose();
    }
    public static void SendSyncRotation(int connectionID, long type, int syncObjID, float x, float y, float z, float w)
    {
        ByteBuffer buffer = new ByteBuffer();

        buffer.WriteLong((long)PacketType.SyncRotation);
        buffer.WriteLong(type);
        buffer.WriteInteger(syncObjID);

        buffer.WriteFloat(x);
        buffer.WriteFloat(y);
        buffer.WriteFloat(z);
        buffer.WriteFloat(w);

        _data.UpdateObject(syncObjID, new Quaternion(x, y, z, w));

        SendDataToAllBut(connectionID, buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendSyncAnimation(int connectionID, byte[] data)
    {
        SendDataToAllBut(connectionID, data);
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
        else
        {
            Console.WriteLine("Constants.GAMERULES returned null, Creating new Properties() now...");
            Constants.GAMERULES = new Properties();
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
