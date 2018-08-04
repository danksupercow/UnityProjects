using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Constants
{
    public static Properties GAMERULES;
    public static readonly string ITEMSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Items");
    public static readonly string GAMERULESPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties.json");
    public static readonly string PLAYERSINFOPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players.json");
}
