using System;
using UnityEngine;

[Serializable]
public class Player
{
    [SerializeField]
    private string _name = "billyTHEkid";

    public string Name { get { return _name; } set { _name = value; General.WriteToPlayerDat(); } }
}
