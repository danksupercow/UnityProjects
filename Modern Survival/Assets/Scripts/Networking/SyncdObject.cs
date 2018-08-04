using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SyncdObject : MonoBehaviour {

    private int id;

    public bool SendInfo = true;
    public bool ReceiveInfo = true;
    public bool SyncPosition;
    public bool SyncRotation;
    public bool SyncSound;
    
    public float positionSendThreshold = 2f;
    public float rotationSendThreshold = 25;

    private Vector3 lastPos;
    public float lastYRot;

    private void Awake()
    {
        lastPos = transform.position;
        lastYRot = transform.rotation.y;
    }

    private void Update()
    {
        if(SyncPosition)
        {
            if (Vector3.Distance(transform.position, lastPos) >= positionSendThreshold)
            {
                //Send Position
                Debug.Log(lastPos + " / " + transform.position);

                lastPos = transform.position;

            }
        }

        if(SyncRotation)
        {
            if (Mathf.Abs(transform.rotation.eulerAngles.y - lastYRot) >= rotationSendThreshold)
            {
                //Send Rotation
                Debug.Log(lastYRot + " / " + transform.rotation.y);

                lastYRot = transform.rotation.eulerAngles.y;
            }
        }

    }

}
