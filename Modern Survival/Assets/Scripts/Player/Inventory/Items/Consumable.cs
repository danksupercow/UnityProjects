using UnityEngine;

[System.Serializable]
public class Consumable : BaseItem
{
    [SerializeField]
    private float _replenishment;
    [SerializeField]
    private string _consumeType;
    [SerializeField]
    private bool _stopsBleeding;

    public Consumable()
    {

    }

    public Consumable(string slug, string name, string desc, string iconpath, int id, int weight, int maxStack, float replenishment, string consumeType, bool stopsBleeding)
    {
        _slug = slug;
        _name = name;
        _description = desc;
        _iconPath = iconpath;
        _id = id;
        _weight = weight;
        _maxStack = maxStack;
        _type = ItemType.Consumable;

        _replenishment = replenishment;
        _consumeType = consumeType;
        _stopsBleeding = stopsBleeding;
    }

    public override void Use(ViewController ply)
    {
        ply.stats.Heal(_replenishment);
        if(ply.stats.isBleeding)
        {
            ply.stats.isBleeding = !_stopsBleeding;
        }
    }


}
