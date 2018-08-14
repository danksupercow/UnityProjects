using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Prop : MonoBehaviour
{
    public AudioClip[] impactSounds;
    private AudioSource audioSource;

    private Collider lastHit;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if(impactSounds.Length > 0)
        {
            audioSource.PlayOneShot(impactSounds[Random.Range(0, impactSounds.Length)]);
        }
    }
}
