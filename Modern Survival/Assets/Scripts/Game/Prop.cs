using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Prop : MonoBehaviour
{
    public AudioClip[] impactSounds;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(impactSounds[Random.Range(0, impactSounds.Length)]);
    }
}
