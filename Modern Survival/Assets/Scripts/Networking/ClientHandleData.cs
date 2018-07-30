using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientHandleData : MonoBehaviour
{
    public static ByteBuffer playerBuffer;
    private delegate void Packet_(byte[] data);
    private static Dictionary<long, Packet_> packets;
    private static long pLength;

    private void Awake()
    {
        InitMessages();
    }

    public static void InitMessages()
    {
        Debug.Log("[Client] Initializing network messages...");
        packets = new Dictionary<long, Packet_>();
        packets.Add((long)PacketType.PlayerJoined, PACKET_PlayerJoined);
        packets.Add((long)PacketType.PlayerData, PACKET_PlayerData);
        packets.Add((long)PacketType.PlayerMove, PACKET_PlayerMove);
        Debug.Log("[Client] Network messages successfully initialized.");
    }

    public static void HandleData(byte[] data)
    {
        byte[] Buffer;
        Buffer = (byte[])data.Clone();

        if (playerBuffer == null) playerBuffer = new ByteBuffer();
        playerBuffer.WriteBytes(Buffer);

        if (playerBuffer.Count() == 0)
        {
            playerBuffer.Clear();
            return;
        }

        if (playerBuffer.Length() >= 8)
        {
            pLength = playerBuffer.ReadLong(false);
            if (pLength <= 0)
            {
                playerBuffer.Clear();
                return;
            }
        }

        if (playerBuffer.Length() >= 8)
        {
            pLength = playerBuffer.ReadLong(false);
            if (pLength <= 0)
            {
                playerBuffer.Clear();
                return;
            }
        }

        while (pLength > 0 & pLength <= playerBuffer.Length() - 8)
        {
            if (pLength <= playerBuffer.Length() - 8)
            {
                playerBuffer.ReadLong();
                data = playerBuffer.ReadBytes((int)pLength);
                HandleDataPackets(data);
            }
            pLength = 0;

            if (playerBuffer.Length() >= 8)
            {
                pLength = playerBuffer.ReadLong(false);
                if (pLength < 0)
                {
                    playerBuffer.Clear();
                    return;
                }
            }
        }
    }

    public static void HandleDataPackets(byte[] data)
    {
        long packetnum; ByteBuffer buffer; Packet_ packet;

        buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        packetnum = buffer.ReadLong();
        buffer = null;

        if (packetnum == 0) return;

        if (packets.TryGetValue(packetnum, out packet))
        {
            packet.Invoke(data);
        }

    }

    //Done Ish
    private static void PACKET_PlayerJoined(byte[] data)
    {
        ByteBuffer buffer;
        long packetnum;
        buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        packetnum = buffer.ReadLong();
        NetworkManager.instance.connectionID = buffer.ReadInteger();
        string s = buffer.ReadString();
        Debug.Log(s);
        buffer.Dispose();
    }

    private static void PACKET_PlayerData(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        int connectionID = buffer.ReadInteger();

        NetworkManager.instance.InstantiatePlayer(connectionID);

        buffer.Dispose();
    }

    private static void PACKET_PlayerMove(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();

        int connectionID = buffer.ReadInteger();

        if (connectionID == NetworkManager.instance.connectionID)
            return;

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();

        float rotX = buffer.ReadFloat();
        float rotY = buffer.ReadFloat();
        float rotZ = buffer.ReadFloat();

        GameObject player = NetworkManager.instance.GetPlayerObjectFromID(connectionID);
        player.transform.position = new Vector3(x,y,z);
        player.transform.eulerAngles = new Vector3(General.WrapAngle(rotX), General.WrapAngle(rotY), General.WrapAngle(rotZ));

        buffer.Dispose();
    }

}
