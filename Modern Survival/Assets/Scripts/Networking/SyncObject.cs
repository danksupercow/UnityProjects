using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NetworkObject))]
public abstract class SyncObject : MonoBehaviour
{
    [Serializable]
    public struct SyncData
    {
        [HideInInspector]
        public NetworkObject netObj;
        [Range(0.1f, 5f)]
        public float sendThreshold;
        [Range(0f, 1f)]
        public float sendDelay;
        [HideInInspector]
        public SyncPosition positionSync;
        [HideInInspector]
        public SyncRotation rotationSync;
        [HideInInspector]
        public SyncAnimator animatorSync;
    }

    public SyncData syncData;
    private float sendTimer;

    public void Init()
    {
        syncData.netObj = GetComponent<NetworkObject>();
        syncData.positionSync = GetComponent<SyncPosition>();
        syncData.rotationSync = GetComponent<SyncRotation>();
        syncData.animatorSync = GetComponent<SyncAnimator>();
        if (syncData.positionSync != null)
            syncData.positionSync.Init();
        if (syncData.rotationSync != null)
            syncData.rotationSync.Init();
        if (syncData.animatorSync != null)
            syncData.animatorSync.Init();
    }

    private void Update()
    {
        Sync();

        if (sendTimer < syncData.sendDelay)
        {
            sendTimer += Time.deltaTime;
            return;
        }

        Send();
    }

    protected abstract void Sync();

    public abstract void Receive(object obj);

    protected abstract void Send();
}
