using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class ExplosiveBase : MonoBehaviour {

    [Header("Base Explosive Variables: ")]
    public float Damage = 100;
    public float Force = 500;
    public float Radius = 10;
    public float explosionDelay = 5;
    public string useButton = "Fire1";
    public bool dealDamage = true;
    public ParticleSystem explosionEffect;
    public AudioClip explosionSound;
    protected MeshRenderer graphic;

    protected RaycastHit hit;
    public new AudioSource audio;
    [HideInInspector]
    public bool isExploding;

    public virtual void CallStart()
    {
        audio = GetComponent<AudioSource>();
        graphic = GetComponentInChildren<MeshRenderer>();
    }

    protected virtual void Explode()
    {
        graphic.enabled = false;

        explosionEffect.Play();

        if(audio != null && explosionSound != null)
        {
            audio.PlayOneShot(explosionSound);
        }

        Collider[] cols = Physics.OverlapSphere(transform.position, Radius);
        for (int i = 0; i < cols.Length; i++)
        {
            Rigidbody rb = cols[i].GetComponent<Rigidbody>();
            
            if(rb != null)
            {
                if (Physics.Raycast(transform.position, (cols[i].transform.position - transform.position), out hit))
                {
                    rb.AddForceAtPosition((cols[i].transform.position - transform.position) * Force, hit.point);
                }
            }

            if(dealDamage)
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

    public void CallUpdate()
    {
        CheckInput();
    }

    protected virtual void CheckInput()
    {
        if (isExploding)
            return;

        bool primaryPressed;

        primaryPressed = Input.GetButtonDown(useButton);

        if (primaryPressed)
        {
            isExploding = true;
            Invoke("Explode", explosionDelay);
        }
    }
}
