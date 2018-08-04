using System;
using UnityEngine;

[Serializable]
public class BaseItem
{
    [SerializeField]
    protected string _slug;
    [SerializeField]
    protected string _name;
    [SerializeField]
    protected string _description;

    [SerializeField]
    protected string _iconPath;

    [SerializeField]
    protected int _id;
    [SerializeField]
    protected float _weight;
    [SerializeField]
    protected int _maxStack;
    [SerializeField]
    protected string _type;

    public string Slug { get { return _slug; } }
    public string Name { get { return _name; } }
    public string Description { get { return _description; } }

    public string IconPath { get { return _iconPath; } }

    public int ID { get { return _id; } }
    public float Weight { get { return _weight; } }
    public int MaxStack { get { return _maxStack; } }
    public string Type { get { return _type; } }

    public BaseItem()
    {
        _name = "empty";
        _description = "null";
        _id = -1;
        _weight = 0;
        _maxStack = 0;
        _type = ItemType.None;
    }

    public BaseItem(string slug, string name, string desc, string iconpath, int id, float weight, int maxStack)
    {
        _slug = slug;
        _name = name;
        _description = desc;
        _iconPath = iconpath;
        _id = id;
        _weight = weight;
        _maxStack = maxStack;
        _type = ItemType.Item;
    }

    public virtual void Use(ViewController ply) { }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public object FromJson(string json)
    {
        return JsonUtility.FromJson(json, GetType());
    }

}