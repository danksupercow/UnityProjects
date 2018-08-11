using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Stats : MonoBehaviour {
    //Local Stats Instance
    public static Stats instance;
    
    public PlayerController isPlayer;
    public bool isDead;
    public bool isBleeding;
    public AudioClip[] hurtClips;
    public AudioClip[] deadClips;
    private AudioClip prevAudio;
    [HideInInspector]
    public AudioSource audioSource;

    private ViewController viewController;
    private float timer;

    //Blood Shit
    public int bleedAmount = 5;
    public int bleedDelay = 2;
    public float bleedThresholdPercent = 0.1f;

    //Survival Variables
    public float currentThirst, maxThirst, currentHunger, maxHunger;
    public float maxHealth;
    public float health;

    public GameObject playerModel;
    public GameObject playerRagdoll;
    public GameObject playerCamera;
    
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

    public void Init()
    {
        this.maxHealth = Game.instance.maxPlayerHealth;
        health = Game.instance.startPlayerHealth;
        viewController = GetComponent<ViewController>();


        if (NetworkManager.connectionID != viewController.connectionID)
            return;

        instance = this;
        isPlayer = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        isPlayer = GetComponent<PlayerController>();
        viewController = GetComponent<ViewController>();
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
        if ((value / maxHealth) >= bleedThresholdPercent)
        {
            isBleeding = true;
        }
        if (audioSource != null)
            PlayRandomOgSound(hurtClips);

        ClientTCP.SendPlayerStats();

        Console.Log("You took " + value + " damage.");
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

    private void Hunger()
    {

    }
    private void Thirst()
    {

    }

    public void UpdateStats(float h, float hu, float t)
    {
        health = h;
        currentHunger = hu;
        currentThirst = t;
    }

    public void Die()
    {
        isDead = true;
        if (audioSource != null)
            PlayRandomOgSound(deadClips);
        if (!isPlayer)
        {
            Debug.LogError("Code Not Implemented Yet!");
        }
        else
        {
            GetComponent<PlayerController>().enabled = false;
        }

        playerModel.SetActive(false);
        playerRagdoll.SetActive(true);
        playerCamera.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
        this.enabled = false;
    }
    public void Reset()
    {
        isDead = false;
        isBleeding = false;
        SetHealth(maxHealth);
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
