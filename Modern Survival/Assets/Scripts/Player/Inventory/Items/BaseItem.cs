using UnityEngine;

[System.Serializable]
public abstract class BaseItem : ScriptableObject
{
    protected string _name;
    protected string _description;
    protected int _id;
    protected int _weight;
    protected int _maxStack;
    protected ItemType _type;

    public string Name { get { return _name; } }
    public string Description { get { return _description; } }
    public int ID { get { return _id; } }
    public int Weight { get { return _weight; } }
    public int MaxStack { get { return _maxStack; } }
    public ItemType Type { get { return _type; } }

    public BaseItem()
    {
        _name = "empty";
        _description = "null";
        _id = -1;
        _weight = 0;
        _maxStack = 0;
        _type = ItemType.None;
    }

    public BaseItem(string name, string desc, int id, int weight, int maxStack)
    {
        _name = name;
        _description = desc;
        _id = id;
        _weight = weight;
        _maxStack = maxStack;
        _type = ItemType.Item;
    }

    public abstract void Use(ViewController ply);

}