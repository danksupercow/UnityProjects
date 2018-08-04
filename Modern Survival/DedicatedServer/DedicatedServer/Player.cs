using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[JsonObject(MemberSerialization.OptIn)]
public class Player
{
    [JsonProperty("Name")]
    private string _name;
    [JsonProperty("UID")]
    private string _uid;
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
    public float Health { get { return _health; } }
    public float Hunger { get { return _hunger; } }
    public float Thirst { get { return _thirst; } }
    public bool JustConnected { get { return _justConnected; } set { _justConnected = value; } }

    public Player() { }

    public Player(string name, string uid, float health, float hunger, float thirst)
    {
        _name = name;
        _uid = uid;
        _health = health;
        _hunger = hunger;
        _thirst = thirst;
    }

    public void Update(float health, float hunger, float thirst)
    {
        _health = health;
        _hunger = hunger;
        _thirst = thirst;
    }
}
