using System.Collections.Generic;
using UnityEngine;

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
        Console.Log("[Client] Initializing network messages...");
        packets = new Dictionary<long, Packet_>();
        packets.Add((long)PacketType.PlayerJoined, PACKET_PlayerJoined);
        packets.Add((long)PacketType.PlayerData, PACKET_PlayerData);
        packets.Add((long)PacketType.PlayerLeft, PACKET_PlayerLeft);
        packets.Add((long)PacketType.ItemData, PACKET_ITEMDATA);
        packets.Add((long)PacketType.GameRules, PACKET_GameRules);
        packets.Add((long)PacketType.Damage, PACKET_Damage);
        packets.Add((long)PacketType.NetSpawn, PACKET_NetSpawn);
        packets.Add((long)PacketType.SyncPosition, PACKET_SyncPosition);
        packets.Add((long)PacketType.SyncRotation, PACKET_SyncRotation);
        packets.Add((long)PacketType.SyncAnimation, PACKET_SyncAnimation);
        Console.Log("[Client] Network messages successfully initialized.");
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
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        long packetnum = buffer.ReadLong();
        int connectionID = buffer.ReadInteger();
        string message = buffer.ReadString();

        NetworkManager.connectionID = connectionID;
        Console.Log(message);

        buffer.Dispose();

    }

    private static void PACKET_PlayerData(byte[] data)
    {
        Console.Log("Recevied Player Data!");
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        int connectionID = buffer.ReadInteger();

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();

        float rotX = buffer.ReadFloat();
        float rotY = buffer.ReadFloat();
        float rotZ = buffer.ReadFloat();
        float rotW = buffer.ReadFloat();
        GameObject ply = NetworkManager.FetchSpawnedPrefab(connectionID);
        if(ply == null && NetworkManager.connectionID == -1)
        {
            ply = NetworkManager.SpawnRegisteredPrefab("local_player", connectionID).gameObject;
        }
        else if(ply == null && NetworkManager.connectionID != -1)
        {
            ply = NetworkManager.SpawnRegisteredPrefab("player", connectionID).gameObject;
        }
        ply.transform.position = new Vector3(x, y, z);
        ply.transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
    }

    private static void PACKET_PlayerLeft(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        long packetnum = buffer.ReadLong();
        int leftID = buffer.ReadInteger();
        string reason = buffer.ReadString();

        if(reason == string.Empty)
        {
            reason = "None.";
        }

        if(leftID == NetworkManager.connectionID)
        {
            Console.Log("You have been disconnected from the server. Reason: " + reason);
            NetworkManager.CleanupScene();
        }
        else
        {
            //Possibly implement sleepers later...
            NetworkManager.DestroySyncdPrefab(leftID);
            Console.Log("Client " + leftID + " has been disconnected from the server. Reason: " + reason);
        }

    }
    
    private static void PACKET_PlayerStats(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        int connectionID = buffer.ReadInteger();

        float health = buffer.ReadFloat();
        float hunger = buffer.ReadFloat();
        float thirst = buffer.ReadFloat();
    }

    private static void PACKET_ITEMDATA(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        string itemJson = buffer.ReadString();

        InventoryManager.RegisterItem(General.FromJsonString(itemJson));

        buffer.Dispose();
    }

    private static void PACKET_GameRules(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        float maxHealth = buffer.ReadFloat();
        float startHealth = buffer.ReadFloat();
        
        Game.instance.maxPlayerHealth = maxHealth;
        Game.instance.startPlayerHealth = startHealth;

        buffer.Dispose();
    }

    private static void PACKET_Damage(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        int id = buffer.ReadInteger();
        float dmg = buffer.ReadFloat();

        //NetworkManager.GetPlayerObjectFromID(id).GetComponent<Stats>().Damage(dmg);

        buffer.Dispose();
    }

    private static void PACKET_NetSpawn(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        int assignedID = buffer.ReadInteger();

        string slug = buffer.ReadString();

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();

        float rotX = buffer.ReadFloat();
        float rotY = buffer.ReadFloat();
        float rotZ = buffer.ReadFloat();
        float rotW = buffer.ReadFloat();
        
        Vector3 pos = new Vector3(x, y, z);
        Quaternion rot = new Quaternion(rotX, rotY, rotZ, rotW);

        Transform t = NetworkManager.SpawnRegisteredPrefab(slug, assignedID).transform;
        t.position = pos;
        t.rotation = rot;
    }

    private static void PACKET_SyncPosition(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        long packetnum = buffer.ReadLong();
        NetworkObjectType type = (NetworkObjectType)buffer.ReadLong();
        int syncObjID = buffer.ReadInteger();

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();

        NetworkManager.UpdateSyncdObject(syncObjID, new Vector3(x, y, z));

        buffer.Dispose();
    }

    private static void PACKET_SyncRotation(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        long packetnum = buffer.ReadLong();
        NetworkObjectType type = (NetworkObjectType)buffer.ReadLong();
        int syncObjID = buffer.ReadInteger();

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();
        float w = buffer.ReadFloat();

        NetworkManager.UpdateSyncdObject(syncObjID, new Quaternion(x, y, z, w));

        buffer.Dispose();
    }

    private static void PACKET_SyncAnimation(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        long packetnum = buffer.ReadLong();
        AnimationType animationType = (AnimationType)buffer.ReadLong();
        int syncID = buffer.ReadInteger();
        string id = buffer.ReadString();
        
        switch (animationType)
        {
            case AnimationType.Bool:
                int i = buffer.ReadInteger();
                bool b = (i == 1);
                NetworkManager.UpdateSyncdObject(syncID, id, b);
                break;
            case AnimationType.Float:
                float f = buffer.ReadFloat();
                NetworkManager.UpdateSyncdObject(syncID, id, f);
                break;
            case AnimationType.Trigger:
                NetworkManager.UpdateSyncdObject(syncID, id);
                break;
            default:
                break;
        }
        
        buffer.Dispose();
    }

}
