using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moped : MonoBehaviour {

    public WheelCollider frontWheel;
    public WheelCollider rearWheel;

    public float maxTurnDegree = 25f;

    private void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            rearWheel.motorTorque -= 5f;
        }
        else
        {
            rearWheel.motorTorque = 0f;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            frontWheel.transform.rotation = new Quaternion(0, 0, maxTurnDegree, 0);
        }
        else if(!Input.GetKey(KeyCode.RightArrow))
        {
            frontWheel.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            frontWheel.transform.rotation = new Quaternion(0, 0, -maxTurnDegree, 0);
        }
    }

}
