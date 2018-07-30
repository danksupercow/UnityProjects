using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Stats : MonoBehaviour {
    public static Stats instance;

    public float health;
    public PlayerController isPlayer;
    public bool isDead;
    public bool isBleeding;
    public AudioClip[] hurtClips;
    public AudioClip[] deadClips;
    private AudioClip prevAudio;
    [HideInInspector]
    public AudioSource audioSource;
    private float timer;

    //Blood Shit
    public int bleedAmount = 5;
    public int bleedDelay = 2;
    public float bleedThresholdPercent = 0.1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isPlayer = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        if(isPlayer)
        {
            //health = Game.instance.maxPlayerHealth;
        }
    }
    private void Update()
    {
        if(isBleeding)
        {
            timer += Time.deltaTime;
            if(timer >= bleedDelay)
            {
                BloodDamage();
                timer = 0f;
            }
        }
    }
    public void Damage(float value)
    {
        if (isDead)
            return;

        health -= value;
        if (health <= 0)
        {
            Die();
            return;
        }
        if ((value / Game.instance.maxPlayerHealth) >= bleedThresholdPercent)
        {
            isBleeding = true;
        }
        if (audioSource != null)
            PlayRandomOgSound(hurtClips);
    }
    public void SetHealth(float value)
    {
        if (value <= 0)
            health = 1;

        health = value;
    }
    private void BloodDamage()
    {
        health -= bleedAmount;
        if(health <= 0)
        {
            Die();
        }
    }
    public void Heal(float value)
    {
        health += value;
    }
    public void Die()
    {
        isDead = true;
        if (audioSource != null)
            PlayRandomOgSound(deadClips);

        transform.Rotate(new Vector3(0, 0, 90));
        if (!isPlayer)
        {
            Debug.LogError("Code Not Implemented Yet!");
        }
        else
        {
            GetComponent<PlayerController>().enabled = false;
        }
        GetComponent<Stats>().enabled = false;
    }
    public void Reset()
    {
        isDead = false;
        isBleeding = false;
        SetHealth(Game.instance.maxPlayerHealth);
    }

    private void PlayRandomOgSound(AudioClip[] clips)
    {
        if (clips.Length < 1)
            return;

        AudioClip ac = clips[Random.Range(0, clips.Length)];
        while (ac == prevAudio && clips.Length > 1)
        {
            ac = clips[Random.Range(0, clips.Length)];
        }

        audioSource.PlayOneShot(ac);
        prevAudio = ac;
    }

}
