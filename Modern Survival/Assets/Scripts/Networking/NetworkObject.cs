using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    [Serializable]
    public struct Data
    {
        public int ID;
        public NetworkObjectType type;
        [HideInInspector]
        public SyncPosition positionSync;
        [HideInInspector]
        public SyncRotation rotationSync;
    }

    public Data data;
}
