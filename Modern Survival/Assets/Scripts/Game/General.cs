using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class General
{
    public static readonly string ItemsPath = Path.Combine(Application.streamingAssetsPath, "Items");
    public static readonly string PlayerIDPath = Path.Combine(Application.streamingAssetsPath, "player.dat");

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    public static void CreateJsonFileFromItem(BaseItem item)
    {
        string path = Path.Combine(ItemsPath, item.Slug + ".json");

        FileStream fs = new FileStream(path, FileMode.Create);
        File.WriteAllText(path, item.ToJson());
    }
    
    public static BaseItem FromJsonString(string jsonItem)
    {
        return new BaseItem().FromJson(jsonItem) as BaseItem; ;
    }

    public static Sprite FromURL(string url)
    {
        WWW www = new WWW(url);

        while(!www.isDone)
        {
            
        }

        Sprite s = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

        return s;
    }

    public static void WriteToPlayerDat()
    {
        Stream stream = File.OpenWrite(PlayerIDPath);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(stream, NetworkManager.PlayerServers);
        stream.Close();
    }

    public static List<ServerData> ReadPlayerData()
    {
        List<ServerData> sv;

        Stream stream = File.Open(PlayerIDPath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        sv = bf.Deserialize(stream) as List<ServerData>;
        stream.Close();

        return sv;
    }

}
