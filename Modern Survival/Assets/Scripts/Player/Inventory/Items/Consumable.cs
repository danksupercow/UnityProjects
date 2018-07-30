using UnityEngine;

[System.Serializable]
public abstract class Consumable : BaseItem
{
    private float _replenishment;

    public Consumable(string name, string desc, int id, int weight, int maxStack, float replenishment)
    {
        _name = name;
        _description = desc;
        _id = id;
        _weight = weight;
        _maxStack = maxStack;
        _type = ItemType.Consumable;
        _replenishment = replenishment;
    }

    public override void Use(ViewController ply)
    {
        ply.stats.Heal(_replenishment);
    }
}
