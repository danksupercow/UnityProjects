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

        return null;
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

}

[JsonObject(MemberSerialization.OptIn)]
public class Player
{
    [JsonProperty("Name")]
    private string _name;
    [JsonProperty("UID")]
    private string _uid;
    [JsonProperty("PositionX")]
    private float _posX;
    [JsonProperty("PositionY")]
    private float _posY;
    [JsonProperty("PositionZ")]
    private float _posZ;
    [JsonProperty("Health")]
    private float _health;
    [JsonProperty("Hunger")]
    private float _hunger;
    [JsonProperty("Thirst")]
    private float _thirst;
    [JsonProperty("JustConnected")]
    private bool _justConnected;

    //Add Inventory Shit Later...

    public string Name { get { return _name; } }
    public string UID { get { return _uid; } }

    public float PosX { get { return _posX; } }
    public float PosY { get { return _posY; } }
    public float PosZ { get { return _posZ; } }

    public float Health { get { return _health; } }
    public float Hunger { get { return _hunger; } }
    public float Thirst { get { return _thirst; } }
    public bool JustConnected { get { return _justConnected; } set { _justConnected = value; } }

    public Player()
    {
        _health = Constants.GAMERULES.PLAYER_STARTING_HEALTH;
    }

    public Player(string name, float x, float y, float z, string uid, float health, float hunger, float thirst)
    {
        _name = name;
        _posX = x;
        _posY = y;
        _posZ = z;
        _uid = uid;
        _health = health;
        _hunger = hunger;
        _thirst = thirst;
    }

    public void UpdateTransform(float x, float y, float z)
    {
        _posX = x;
        _posY = y;
        _posZ = z;

        General.WritePlayersInfo();
    }

    public void UpdateStats(float health, float hunger, float thirst)
    {
        _health = health;
        _hunger = hunger;
        _thirst = thirst;

        General.WritePlayersInfo();
    }

    public void UpdateName(string name)
    {
        Console.WriteLine(_name + " changed their name to " + name);
        _name = name;
    }
}
