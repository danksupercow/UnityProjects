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

public struct ItemType
{
    public const string None = "none";
    public const string Item = "item";
    public const string Consumable = "consumable";
    public const string Wearable = "wearable";
    public const string Component = "component";
    public const string Resource = "resource";
}

public struct ConsumeType
{
    public const string Health = "health";
    public const string Hunger = "hunger";
    public const string Thirst = "thirst";
}

public enum WeaponHoldType
{
    None,
    Handgun,
    Rifle
}

public enum AmmoType
{
    NineMM
}

public enum NetworkObjectType
{
    Player,
    SyncdObject
}

public enum AnimationType
{
    Bool = 1,
    Float,
    Trigger
}