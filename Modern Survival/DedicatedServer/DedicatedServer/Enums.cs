using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PacketType
{
    PlayerJoined = 1,
    PlayerLeft,
    PlayerData,
    PlayerMove,
    PlayerStats,
    PlayerPing,
    SyncdObject,
    ItemData,
    GameRules,
    Damage,
    NetSpawn,
    SyncPosition,
    SyncRotation,
    SyncAnimation
}

public enum AnimationType
{
    Bool = 1,
    Float,
    Trigger
}