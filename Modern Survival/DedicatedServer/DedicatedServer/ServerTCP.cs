using System;
using System.Net;
using System.Net.Sockets;

namespace DedicatedServer
{
    class ServerTCP
    {
        public static TcpListener serverSocket;

        public static Client[] Clients = new Client[Constants.MAX_PLAYERS];

        public static void InitNetwork()
        {
            serverSocket = new TcpListener(IPAddress.Any, 5555);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
            Console.WriteLine("Server has successfully started.");
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

            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if(Clients[i].socket == null)
                {
                    Clients[i] = new Client(client, i, client.Client.RemoteEndPoint.ToString());
                    Console.WriteLine("Connection received from " + Clients[i].ip);
                    SendWelcomeMessage(i);
                    Program.JoinGame(i);
                    return;
                }
            }
        }

        public static void SendDataTo(long index, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            Clients[index].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer = null;
        }
        
        private static void SendDataToAll(byte[] data)
        {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if(Clients[i].socket != null)
                {
                    SendDataTo(i, data);
                }
            }
        }

        public static void SendWelcomeMessage(long connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)PacketType.PlayerJoined);
            buffer.WriteInteger((int)connectionID);
            buffer.WriteString("[Server] Welcome To The Server!");

            SendDataTo(connectionID, buffer.ToArray());
            //SendJoinMap(index);
            buffer.Dispose();
        }

        public static byte[] PlayerData(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)PacketType.PlayerData);
            buffer.WriteInteger(connectionID);
            return buffer.ToArray();
        }

        public static void SendInWorld(int connectionID)
        {
            //Send all players on current scene to the player itself
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if(Clients[i].socket != null)
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
        }

    }
}
