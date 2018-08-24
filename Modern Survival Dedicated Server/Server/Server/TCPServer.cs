using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class TCPServer
    {
        public static TcpListener serverSocket;
        public static Client[] clients;

        public static void Init()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            serverSocket = new TcpListener(IPAddress.Any, 11866);
            serverSocket.Start();

            clients = new Client[Properties.maxPlayers];

            serverSocket.BeginAcceptTcpClient(ClientConnected, null);
            sw.Stop();
            Debug.Log("Server Started Successfully In " + (sw.ElapsedMilliseconds/1000) + "s");
        }

        public static void Close()
        {
            for (int i = 0; i < clients.Length; i++)
            {
                clients[i].Close("Server Shutdown!");
            }
        }

        private static void ClientConnected(IAsyncResult result)
        {
            try
            {
                TcpClient client = serverSocket.EndAcceptTcpClient(result);
                serverSocket.BeginAcceptTcpClient(ClientConnected, null);
                client.NoDelay = false;

                for (int i = 0; i < Properties.maxPlayers; i++)
                {
                    if(clients[i].socket == null)
                    {
                        clients[i] = new Client(client, i, client.Client.RemoteEndPoint.ToString());
                        Debug.LogUpdate("Player Joined The Server On IP: " + clients[i].ip);
                        //Tell Client The Connected Successfully
                        SendConnectionSuccessfull(i);
                        //Send The Client Their ConnectionID
                        SendDataTo(i, PlayerData(i));
                        //Send The Client All Of The Other Connected Players
                        SendPlayersInWorld(i);
                        //Send Initial Player Data To Connected Player
                        //Send All Other SyncdObjects to Player(other players, networked prefabs)
                        return;
                    }
                }

                #region TellAttemptingClientServerIsFull
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteLong((long)PacketType.ServerFull);
                byte[] data = buffer.ToArray();
                client.GetStream().BeginWrite(data, 0, data.Length, null, null);
                #endregion
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public static byte[] PlayerData(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)PacketType.PlayerData);
            buffer.WriteInteger(connectionID);
            return buffer.ToArray();
        }

        #region DataSendingMethods
        public static void SendDataTo(long connectionID, byte[] data)
        {
            try
            {
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
                buffer.WriteBytes(data);
                clients[connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
                buffer = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static void SendDataToAll(byte[] data)
        {
            for (int i = 0; i < Properties.maxPlayers; i++)
            {
                if (clients[i].socket != null)
                {
                    SendDataTo(i, data);
                }
            }
        }
        private static void SendDataToAllBut(int connectionId, byte[] data)
        {
            for (int i = 0; i < Properties.maxPlayers; i++)
            {
                if (connectionId != i)
                    if (clients[i].socket != null)
                        SendDataTo(i, data);
            }
        }
        #endregion

        #region Initial Connection Stuff
        public static void SendConnectionSuccessfull(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)PacketType.SuccessfullConnection);
            buffer.WriteString(Properties.welcomeMessage);
            SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void SendPlayersInWorld(int connectionID)
        {
            for (int i = 0; i < Properties.maxPlayers; i++)
            {
                if(i != connectionID)
                {
                    SendDataTo(connectionID, PlayerData(i));
                }
            }
        }
        public static void SendSpawnedPrefabs(int connectionID)
        {
            ByteBuffer buffer;
            for (int i = 0; i < ServerData.SpawnedPrefabCount; i++)
            {
                buffer = new ByteBuffer();
                buffer.WriteLong((long)PacketType.SyncSpawn);
                buffer.WriteString(ServerData.spawnedPrefabs[i].slug);
                buffer.WriteInteger(ServerData.spawnedPrefabs[i].syncID);

                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.position.x);
                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.position.y);
                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.position.z);

                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.rotation.x);
                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.rotation.y);
                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.rotation.z);
                buffer.WriteFloat(ServerData.spawnedPrefabs[i].transform.rotation.w);

                SendDataTo(connectionID, buffer.ToArray());
                buffer.Dispose();
            }
        }
        #endregion
    }
}
