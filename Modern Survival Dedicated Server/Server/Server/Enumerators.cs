using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum PacketType
    {
        ServerFull = 1,
        SuccessfullConnection,
        PlayerConnected,
        PlayerData,
        Ping,
        SyncPosition,
        SyncRotation,
        SyncAnimator,
        DamageSyncObject,
        SyncSpawn
    }
}
