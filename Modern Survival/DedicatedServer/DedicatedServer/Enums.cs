using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    public enum PacketType
    {
        PlayerJoined = 1,
        PlayerData,
        PlayerMove,
        PlayerPing,
        SyncdObjectMove
    }
}
