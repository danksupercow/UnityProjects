using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePlastic : ExplosiveBase
{
    [Header("Plastic Explosives Variables")]
    public GameObject explosivePrefab;
    public float tossDistance = 10f;
    private ExplosiveBase explosive;

    protected override void CheckInput()
    {
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
                    //s.Damage(Damage);
                }
            }
        }

        Destroy(gameObject, explosionSound.length);
    }

    protected void Toss()
    {
        bool primaryPressed;

        primaryPressed = Input.GetButtonDown(useButton);
        RaycastHit hit;
        Quaternion fireRotation = Quaternion.LookRotation(transform.forward);

        if (primaryPressed)
        {
            if (Physics.Raycast(ViewController.playerRay, out hit, tossDistance))
            {
                Transform t = Instantiate(explosivePrefab).transform;
                t.rotation = Quaternion.LookRotation(hit.normal);
                t.position = hit.point;
                t.SetParent(hit.transform);
                explosive = t.GetComponent<ExplosiveBase>();
                explosive.CallStart();
                explosive.isExploding = true;
                explosive.transform.SetParent(null);
                explosive.Invoke("Explode", explosionDelay);
            }
        }

    }
}
