﻿using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientTCP
{
    private const int maxBufferSize = 4096;

    public TcpClient playerSocket;
    private bool connecting;
    private bool connected;
    private static NetworkStream myStream;
    private byte[] asyncBuff;

    private static string currentIP;
    private static int currentPort = -1;

    public static string CurrentIP { get { return currentIP; } }
    public static int CurrentPort { get { return currentPort; } }

    public void Connect()
    {
        Connect("127.0.0.1", 5555);
    }

    public void Connect(string ip, int port)
    {
        playerSocket = new TcpClient();
        playerSocket.ReceiveBufferSize = maxBufferSize;
        playerSocket.SendBufferSize = maxBufferSize;
        playerSocket.NoDelay = false;
        asyncBuff = new byte[8192];
        playerSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), playerSocket);

        currentIP = ip;
        currentPort = port;

        connecting = true;
    }

    public void Disconnect()
    {
        currentIP = string.Empty;
        currentPort = -1;
        if(playerSocket != null)
        {
            playerSocket.Close();
        }
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            playerSocket.EndConnect(ar);
            if(playerSocket.Connected == false)
            {
                currentIP = string.Empty;
                currentPort = -1;
                connected = false;
                connecting = false;
                Console.Log("Failed to connect to server at " + currentIP + ":" + currentPort);
                return;
            }
            else
            {
                playerSocket.NoDelay = true;
                myStream = playerSocket.GetStream();
                myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
                NetworkManager.RegisterServer(currentIP, currentPort);
                connected = true;
                connecting = false;
                SendPlayerData();
                Console.Log("[Client] Successfully connected server at " + currentIP + ":" + currentPort);

            }
        }
        catch(Exception e)
        {
            currentIP = string.Empty;
            currentPort = -1;
            connecting = false;
            connected = false;
            Console.LogError(e);
        }
    }

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            int byteAmt = myStream.EndRead(ar);
            byte[] myBytes = new byte[byteAmt];
            Buffer.BlockCopy(asyncBuff, 0, myBytes, 0, byteAmt);
            if (byteAmt == 0) return;

            UnityThread.executeInUpdate(() =>
            {
                ClientHandleData.HandleData(myBytes);
            });
            myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
        }
        catch
        {
            Console.LogError("Connection to Server " + currentIP + ":" + currentPort + " Was Lost.");
            currentIP = string.Empty;
            currentPort = -1;
            connected = false;
            connecting = false;
            NetworkManager.DestroyAllPlayers();
        }
    }

    public static void SendData(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
        buffer.WriteBytes(data);
        myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
    }

    public static void SendPlayerData()
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerData);

        ServerData sd = NetworkManager.FetchServerData(currentIP, currentPort);

        buffer.WriteString(sd.UID);
        buffer.WriteString(sd.Name);

        SendData(buffer.ToArray());
    }

    public static void SendMovement(Vector3 pos, Quaternion rot)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerMove);
        
        //position
        buffer.WriteFloat(pos.x);
        buffer.WriteFloat(pos.y);
        buffer.WriteFloat(pos.z);

        //rotation
        buffer.WriteFloat(General.UnwrapAngle(rot.x));
        buffer.WriteFloat(General.UnwrapAngle(rot.y));
        buffer.WriteFloat(General.UnwrapAngle(rot.z));

        //Sending Data
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendPlayerStats()
    {
        Stats s = Stats.instance;

        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.PlayerStats);

        buffer.WriteFloat(s.health);
        buffer.WriteFloat(s.currentHunger);
        buffer.WriteFloat(s.currentThirst);

        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendDamage(int connectionID, float damage)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.Damage);
        buffer.WriteInteger(connectionID);
        buffer.WriteFloat(damage);

        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SpawnRegisteredPrefab(string slug, Vector3 pos, Quaternion rot)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteLong((long)PacketType.NetSpawn);

        //Send Server The 'slug' of the registered prefab you want to spawn...
        buffer.WriteString(slug);

        //Send Server the position you want to spawn the registered prefab at...
        buffer.WriteFloat(pos.x);
        buffer.WriteFloat(pos.y);
        buffer.WriteFloat(pos.z);

        //Send Server The rotation of the object you want to spawn...
        buffer.WriteFloat(General.UnwrapAngle(rot.x));
        buffer.WriteFloat(General.UnwrapAngle(rot.y));
        buffer.WriteFloat(General.UnwrapAngle(rot.z));

        //Finally, Convert ByteBuffer to byte[] and Send to the server then dispose the buffer.
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
}
