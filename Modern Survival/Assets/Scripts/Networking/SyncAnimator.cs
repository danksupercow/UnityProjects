using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class SyncAnimator : MonoBehaviour
{
    public Animator animator;
    public bool doSync = true;
    private NetworkObject networkObject;
    private int syncID;

    public bool oldBool;
    private float oldFloat;

    public void Init()
    {
        networkObject = GetComponent<NetworkObject>();
        syncID = networkObject.data.ID;
    }

    public void SetBool(string id, bool value)
    {
        oldBool = animator.GetBool(id);

        if (oldBool == value)
            return;

        animator.SetBool(id, value);

        if (syncID == -1 || NetworkManager.instance == null || !NetworkManager.Connected)
        {
            return;
        }

        if (doSync)
            ClientTCP.SendAnimationData(syncID, id, value);
    }
    
    public void SetFloat(string id, float value, float dampTime, float deltaTime)
    {
        oldFloat = animator.GetFloat(id);

        if (oldFloat == value)
            return;

        animator.SetFloat(id, value, dampTime, deltaTime);
        oldFloat = value;

        if (syncID == -1 || NetworkManager.instance == null || !NetworkManager.Connected)
        {
            return;
        }
        if (doSync)
            ClientTCP.SendAnimationData(syncID, id, value);
    }

    public void SetTrigger(string id)
    {
        animator.SetTrigger(id);
        if (syncID == -1 || NetworkManager.instance == null || !NetworkManager.Connected)
        {
            return;
        }
        if (doSync)
            ClientTCP.SendAnimationData(syncID, id);
    }

}
