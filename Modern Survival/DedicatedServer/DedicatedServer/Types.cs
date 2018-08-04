using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Types
{
    public static TempPlayerRec[] TempPlayer;

    public struct TempPlayerRec
    {
        public ByteBuffer Buffer;
        public long DataBytes;
        public long DataPackets;
    }
}
