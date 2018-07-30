using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveGrenade : ExplosiveBase
{

    [Header("Grenade Variables: ")]
    public GameObject grenadePrefab;
    public float throwForce = 20;
    [HideInInspector]
    public Rigidbody rb;

    public override void CallStart()
    {
        base.CallStart();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    protected override void CheckInput()
    {
        if (isExploding)
            return;

        Toss();
    }

    protected override void Explode()
    {
        audio.PlayOneShot(explosionSound);
        explosionEffect.Play();
        graphic.enabled = false;

        Collider[] cols = Physics.OverlapSphere(transform.position, Radius);
        for (int i = 0; i < cols.Length; i++)
        {
            Rigidbody rb = cols[i].GetComponent<Rigidbody>();

            if (rb != null)
            {
                if (Physics.Raycast(transform.position, (cols[i].transform.position - transform.position), out hit))
                {
                    rb.AddForceAtPosition((cols[i].transform.position - transform.position) * Force, hit.point);
                }
            }

            if (dealDamage)
            {
                Stats s = cols[i].GetComponent<Stats>();
                if (s != null)
                {
                    s.Damage(Damage);
                }
            }
        }

        Destroy(gameObject, explosionSound.length);
    }

    protected void Toss()
    {
        bool primaryPressed;

        primaryPressed = Input.GetButtonDown(useButton);
        Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
        Vector3 throwDir = fireRotation * Vector3.forward;

        if (primaryPressed)
        {
            Transform t = Instantiate(grenadePrefab).transform;
            t.transform.position = transform.position;
            ExplosiveGrenade explosive = t.GetComponent<ExplosiveGrenade>();
            explosive.CallStart();
            explosive.rb.isKinematic = false;
            explosive.rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
            explosive.isExploding = true;
            explosive.Invoke("Explode", explosionDelay);
            return;
        }

    }

}
