﻿using System;
using System.Net.Sockets;
using UnityEngine;

[Serializable]
public class ServerData
{
    [SerializeField]
    string _address;
    [SerializeField]
    int _port;
    [SerializeField]
    string _uniqueID;
    [SerializeField]
    string _name;
    
    public string Name { get { return _name; } set { _name = value; General.WriteToPlayerDat(); } }
    public string UID { get { return _uniqueID; } }
    public string ServerIP { get { return _address; } }
    public int ServerPort { get { return _port; } }

    public ServerData() { }

    public ServerData(string ip, int port)
    {
        _address = ip;
        _port = port;
        _uniqueID = Guid.NewGuid().ToString();
        _name = "Billy";
    }

}