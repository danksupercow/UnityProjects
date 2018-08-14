using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public float throwForce = 20;
    public GameObject prefab;

    private void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            GameObject g = ObjectPooler.instance.SpawnFromPool("dildo", transform.position, transform.rotation);
            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
            Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
            Vector3 throwDir = fireRotation * Vector3.forward;
            g.GetComponentInChildren<Rigidbody>().AddForce(throwDir * throwForce, ForceMode.Impulse);
        }
    }

}
