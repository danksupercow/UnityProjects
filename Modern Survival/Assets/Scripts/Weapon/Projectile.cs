using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public GameObject prefab;
    public float damage;
    public new Rigidbody rigidbody;
    public delegate void HitCallback(Transform hit, Vector3 pos);
    public HitCallback hitCallback;

    private RaycastHit hit;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hit, 6f))
        {
            hitCallback(hit.transform, hit.point);
            Debug.Log("Hit Something!");
            Destroy(gameObject);
        }
    }

}
