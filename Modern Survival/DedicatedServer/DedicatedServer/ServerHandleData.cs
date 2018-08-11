using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ServerHandleData
{
    private delegate void Packet_(long connectionID, byte[] data);
    private static Dictionary<long, Packet_> packets;
    private static long pLength;

    public static void Init()
    {
        packets = new Dictionary<long, Packet_>();
        packets.Add((long)PacketType.PlayerData, PACKET_PLAYERDATA);
        packets.Add((long)PacketType.PlayerMove, PACKET_PLAYERMOVE);
        packets.Add((long)PacketType.PlayerStats, PACKET_PLAYERSTATS);
        packets.Add((long)PacketType.Damage, PACKET_DAMAGE);
        packets.Add((long)PacketType.NetSpawn, PACKET_NETSPAWN);
    }

    public static void HandleData(long connectionID, byte[] data)
    {
        byte[] Buffer;
        Buffer = (byte[])data.Clone();

        if (Types.TempPlayer[connectionID].Buffer == null) Types.TempPlayer[connectionID].Buffer = new ByteBuffer();
        Types.TempPlayer[connectionID].Buffer.WriteBytes(Buffer);

        if (Types.TempPlayer[connectionID].Buffer.Count() == 0)
        {
            Types.TempPlayer[connectionID].Buffer.Clear();
            return;
        }

        if (Types.TempPlayer[connectionID].Buffer.Length() >= 4)
        {
            pLength = Types.TempPlayer[connectionID].Buffer.ReadLong(false);
            if (pLength <= 0)
            {
                Types.TempPlayer[connectionID].Buffer.Clear();
                return;
            }
        }

        while (pLength > 0 & pLength <= Types.TempPlayer[connectionID].Buffer.Length() - 8)
        {
            if (pLength <= Types.TempPlayer[connectionID].Buffer.Length() - 8)
            {
                Types.TempPlayer[connectionID].Buffer.ReadLong();
                data = Types.TempPlayer[connectionID].Buffer.ReadBytes((int)pLength);
                HandleDataPackets(connectionID, data);
            }
            pLength = 0;

            if (Types.TempPlayer[connectionID].Buffer.Length() >= 4)
            {
                pLength = Types.TempPlayer[connectionID].Buffer.ReadLong(false);
                if (pLength < 0)
                {
                    Types.TempPlayer[connectionID].Buffer.Clear();
                    return;
                }
            }
        }

        if (pLength <= 1)
        {
            Types.TempPlayer[connectionID].Buffer.Clear();
        }
    }

    public static void HandleDataPackets(long connectionID, byte[] data)
    {
        long packetnum; ByteBuffer buffer; Packet_ packet;

        buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        packetnum = buffer.ReadLong();
        buffer = null;

        if (packetnum == 0) return;

        if (packets.TryGetValue(packetnum, out packet))
        {
            packet.Invoke(connectionID, data);
        }

    }

    private static void PACKET_PLAYERDATA(long connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        string uid = buffer.ReadString();
        string name = buffer.ReadString();

        if(ServerTCP.SavedPlayers.GetPlayerFromUID(uid) == null)
        {
            ServerTCP.SavedPlayers.SavePlayer(new Player(uid));
            ServerTCP.Clients[connectionID].player = ServerTCP.SavedPlayers.GetPlayerFromUID(uid);
        }
    }

    private static void PACKET_PLAYERMOVE(long connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();

        float rotX = buffer.ReadFloat();
        float rotY = buffer.ReadFloat();
        float rotZ = buffer.ReadFloat();

        //Update Players Stored Position
        ServerTCP.Clients[connectionID].player.UpdateTransform(x, y, z);

        ServerTCP.SendPlayerMove((int)connectionID, x, y, z, rotX, rotY, rotZ);
        buffer.Dispose();

    }

    private static void PACKET_PLAYERSTATS(long connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        float health = buffer.ReadFloat();
        float hunger = buffer.ReadFloat();
        float thirst = buffer.ReadFloat();

        ServerTCP.Clients[connectionID].player.UpdateStats(health, hunger, thirst);
    }

    private static void PACKET_DAMAGE(long connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetNum = buffer.ReadLong();
        int id = buffer.ReadInteger();
        float dmg = buffer.ReadFloat();

        ServerTCP.SendDamage(id, dmg);

        Console.WriteLine("Player: " + connectionID + " dealt " + dmg + " to " + "Player: " + id);
    }

    private static void PACKET_NETSPAWN(long connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);

        long packetnum = buffer.ReadLong();
        string slug = buffer.ReadString();

        float x = buffer.ReadFloat();
        float y = buffer.ReadFloat();
        float z = buffer.ReadFloat();

        float rotX = buffer.ReadFloat();
        float rotY = buffer.ReadFloat();
        float rotZ = buffer.ReadFloat();

        ServerTCP.SendNetSpawnRequest((int)connectionID, slug, x, y, z, rotX, rotY, rotZ);
        buffer.Dispose();
    }
}
