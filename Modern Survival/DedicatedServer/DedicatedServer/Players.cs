using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[JsonObject(MemberSerialization.OptIn)]
public class Players
{
    [JsonProperty("Players")]
    private List<Player> players = new List<Player>();

    public int Count { get { return players.Count; } }

    public Player GetPlayerFromUID(string uid)
    {
        for (int i = 0; i < Count; i++)
        {
            if(players[i].UID == uid)
            {
                return players[i];
            }
        }

        return default(Player);
    }
    public int GetPlayerIndex(Player ply)
    {
        for (int i = 0; i < Count; i++)
        {
            if(players[i].UID == ply.UID)
            {
                return i;
            }
        }

        return -1;
    }
    public Player SavePlayer(Player ply)
    {
        Console.WriteLine("Ply Saved");

        Player p = GetPlayerFromUID(ply.UID);
        if(p != null)
        {
            p = ply;
        }
        else
        {
            players.Add(ply);
        }

        return p;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

[JsonObject(MemberSerialization.OptIn)]
public struct Player
{
    [JsonProperty("DisplayName")]
    private string _displayName;
    [JsonProperty("UID")]
    private string _uid;

    [JsonProperty("Transform")]
    private Transform _transform;

    [JsonProperty("Health")]
    private float _health;
    [JsonProperty("Hunger")]
    private float _hunger;
    [JsonProperty("Thirst")]
    private float _thirst;


    [JsonProperty("JustConnected")]
    private bool _justConnected;

    //Add Inventory Shit Later...

    public string DisplayName { get { return _displayName; } }
    public string UID { get { return _uid; } set { _uid = value; } }
    
    public Transform Transform { get { return _transform; } }

    public float Health { get { return _health; } }
    public float Hunger { get { return _hunger; } }
    public float Thirst { get { return _thirst; } }

    public bool JustConnected { get { return _justConnected; } set { _justConnected = value; } }

    public void SetTransform(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        _transform.position.x = posX;
        _transform.position.y = posY;
        _transform.position.z = posZ;

        _transform.rotation.x = rotX;
        _transform.rotation.y = rotY;
        _transform.rotation.z = rotZ;
        _transform.rotation.w = rotW;
    }
    
    public void Clear()
    {
        this = default(Player);
    }

    public static bool operator ==(Player p1, Player p2)
    {
        return p1.Equals(p2);
    }

    public static bool operator !=(Player p1, Player p2)
    {
        return !p1.Equals(p2);
    }
}

public struct Vector3
{
    public float x;
    public float y;
    public float z;

    public Vector3(float X, float Y, float Z)
    {
        x = X;
        y = Y;
        z = Z;
    }
}

public struct Quaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public Quaternion(float X, float Y, float Z, float W)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
    }
}

public struct Transform
{
    public Vector3 position;
    public Quaternion rotation;
}
