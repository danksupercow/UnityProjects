using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPosition : SyncObject
{
    [Serializable]
    public struct PositionData
    {
        public bool send;
        public Vector3 lastPosition;
        public Vector3 nextPosition;
        [Tooltip("The speed at which this object moves from current position to next position smoothly. If set to 0 don't lerp.")]
        public float lerpSpeed; //if == 0 then dont lerp
    }

    public PositionData positionData;
    private NetworkObjectType type;

    public void Init()
    {
        syncData.netObj = GetComponent<NetworkObject>();
        type = syncData.netObj.data.type;
    }

    protected override void Sync()
    {
        if (positionData.nextPosition != Vector3.zero)
        {
            if (positionData.lerpSpeed == 0)
            {
                transform.position = positionData.nextPosition;
                positionData.nextPosition = Vector3.zero;
                return;
            }

            transform.position = Vector3.Lerp(transform.position, positionData.nextPosition, positionData.lerpSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, positionData.nextPosition) <= 0.1f)
            {
                positionData.nextPosition = Vector3.zero;
            }
        }
    }

    protected override void Send()
    {
        if (positionData.send == false || Vector3.Distance(positionData.lastPosition, transform.position) < syncData.sendThreshold || NetworkManager.instance == null || !NetworkManager.Connected)
            return;

        ClientTCP.SendSyncPosition(syncData.netObj.data.ID, type, transform.position);
        positionData.lastPosition = transform.position;
    }

    public override void Receive(object obj)
    {
        if (positionData.send && type == NetworkObjectType.Player)
            return;
        positionData.nextPosition = (Vector3)obj;
    }
}
