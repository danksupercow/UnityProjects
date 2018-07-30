using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    public class ServerHandleData
    {
        private delegate void Packet_(long connectionID, byte[] data);
        private static Dictionary<long, Packet_> packets;
        private static long pLength;

        public static bool pingReceived;

        public static void Init()
        {
            packets = new Dictionary<long, Packet_>();
            packets.Add((long)PacketType.PlayerJoined, PACKET_WELCOME);
            packets.Add((long)PacketType.PlayerMove, PACKET_PLAYERMOVE);
        }

        public static void HandleData(long connectionID, byte[] data)
        {
            byte[] Buffer;
            Buffer = (byte[])data.Clone();

            if (Types.TempPlayer[connectionID].Buffer == null) Types.TempPlayer[connectionID].Buffer = new ByteBuffer();
            Types.TempPlayer[connectionID].Buffer.WriteBytes(Buffer);

            if(Types.TempPlayer[connectionID].Buffer.Count() == 0)
            {
                Types.TempPlayer[connectionID].Buffer.Clear();
                return;
            }

            if(Types.TempPlayer[connectionID].Buffer.Length() >= 4)
            {
                pLength = Types.TempPlayer[connectionID].Buffer.ReadLong(false);
                if(pLength <= 0)
                {
                    Types.TempPlayer[connectionID].Buffer.Clear();
                    return;
                }
            }

            while(pLength > 0 & pLength <= Types.TempPlayer[connectionID].Buffer.Length() - 8)
            {
                if(pLength <= Types.TempPlayer[connectionID].Buffer.Length() - 8)
                {
                    Types.TempPlayer[connectionID].Buffer.ReadLong();
                    data = Types.TempPlayer[connectionID].Buffer.ReadBytes((int)pLength);
                    HandleDataPackets(connectionID, data);
                }
                pLength = 0;

                if(Types.TempPlayer[connectionID].Buffer.Length() >= 4)
                {
                    pLength = Types.TempPlayer[connectionID].Buffer.ReadLong(false);
                    if(pLength < 0)
                    {
                        Types.TempPlayer[connectionID].Buffer.Clear();
                        return;
                    }
                }
            }

            if(pLength <= 1)
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

            if(packets.TryGetValue(packetnum, out packet))
            {
                packet.Invoke(connectionID, data);
            }

        }

        private static void PACKET_WELCOME(long connectionID, byte[] data)
        {
            Console.WriteLine("Received Message");
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

            Console.WriteLine("Player " + connectionID + " has moved!");

            ServerTCP.SendPlayerMove((int)connectionID, x, y, z, rotX, rotY, rotZ);

        }

    }
}
