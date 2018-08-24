using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Properties
{
    [JsonProperty("maxplayercount")]
    public int MAX_PLAYER_COUNT = 10;
    [JsonProperty("maxplayerhealth")]
    public float MAX_PLAYER_HEALTH = 500;
    [JsonProperty("playerstartinghealth")]
    public float PLAYER_STARTING_HEALTH = 250;
    [JsonProperty("positionsendthreshold")]
    public float POSITION_SEND_THRESHOLD = 0.5f;
    [JsonProperty("rotationsendthreshold")]
    public float ROTATION_SEND_THRESHOLD = 0.5f;
    [JsonProperty("particlepoolsize")]
    public int PARTICLE_POOL_SIZE = 300;

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public override string ToString()
    {
        return "\nMax Player Count = " + MAX_PLAYER_COUNT + ",\nMax Player Health = " + MAX_PLAYER_HEALTH + ",\nPlayer Starting Health = " + PLAYER_STARTING_HEALTH + "\n";
    }
}
