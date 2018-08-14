using UnityEngine;
using System.Collections;

public class ParticleEffect : MonoBehaviour, IPooledObject
{
    private ParticleSystem particle;
    private new AudioSource audio;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        audio = GetComponent<AudioSource>();
    }

    public void OnObjectSpawn()
    {
        particle.Play();
        if(audio != null)
            audio.Play();
    }
}
