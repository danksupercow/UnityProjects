using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncRotation : SyncObject
{
    [Serializable]
    public struct RotationData
    {
        public bool send;
        public Quaternion lastRotation;
        public Quaternion nextRotation;
        public float lerpSpeed;
    }

    public RotationData rotationData;
    public int receivedCount;
    private NetworkObjectType type;

    public void Init()
    {
        syncData.netObj = GetComponent<NetworkObject>();
        type = syncData.netObj.data.type;
    }

    protected override void Sync()
    {
        if (rotationData.send)
            return;

        if (rotationData.nextRotation != Quaternion.identity)
        {
            if (rotationData.lerpSpeed == 0)
            {
                transform.rotation = rotationData.nextRotation;
                rotationData.nextRotation = Quaternion.identity;
                return;
            }
            
            transform.rotation = new Quaternion(rotationData.nextRotation.x, Mathf.Lerp(transform.rotation.y, rotationData.nextRotation.y, rotationData.lerpSpeed * Time.deltaTime), rotationData.nextRotation.z, rotationData.nextRotation.w);
            if(Quaternion.Angle(transform.rotation, rotationData.nextRotation) <= 2)
            {
                rotationData.nextRotation = Quaternion.identity;
            }
        }
    }

    protected override void Send()
    {
        if (rotationData.send == false || Quaternion.Angle(rotationData.lastRotation, transform.rotation) < syncData.sendThreshold || NetworkManager.instance == null || !NetworkManager.Connected)
            return;

        ClientTCP.SendSyncRotation(syncData.netObj.data.ID, type, transform.rotation);

        rotationData.lastRotation = transform.rotation;
    }

    public override void Receive(object obj)
    {
        if (rotationData.send && type == NetworkObjectType.Player)
            return;

        receivedCount++;

        Quaternion newRot = (Quaternion)obj;
        rotationData.nextRotation = newRot;
    }
}
