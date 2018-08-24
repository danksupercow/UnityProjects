using System;
using UnityEngine;

public struct Player
{
    public GameObject gameObject;
    public NetworkObject networkObject;
    public SyncPosition syncPosition;
    public SyncRotation syncRotation;
    public SyncAnimator syncAnimator;

    public void Init(int assignedID)
    {
        networkObject.data.ID = assignedID;
        syncAnimator.Init();
    }
    
}
