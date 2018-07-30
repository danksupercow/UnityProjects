using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    class Types
    {
        public static TempPlayerRec[] TempPlayer = new TempPlayerRec[Constants.MAX_PLAYERS];

        public struct TempPlayerRec
        {
            public ByteBuffer Buffer;
            public long DataBytes;
            public long DataPackets;
        }
    }
}
