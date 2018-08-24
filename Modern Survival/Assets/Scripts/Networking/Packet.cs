using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Packet
{
    public byte[] data;
    public Packet(byte[] _data)
    {
        data = _data;
    }
}
