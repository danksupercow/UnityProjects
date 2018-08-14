using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Stats : MonoBehaviour
{
    public static Stats instance;

    //Constant/Maximum Variables
    private float _maxHealth = 100;
    private float _maxBlood = 1000;
    private float _maxHunger = 300;
    private float _maxThirst = 300;
    private int _maxBloodLossDelay = 10;

    //Survival Variables
    private float _blood;
    private float _hunger;
    private float _thirst;
    private bool _bleeding;

    //The Amount of time after variable has reached max before they are depleted again
    private int t_thirstDelay = 180;
    private int t_hungerDelay = 300;
    //idk
    private int t_thirstDep = 10;
    private int t_hungerDep = 20;
    //The amount of time after blood has been taken away before it can regenerate again;
    private int t_bloodDelay = 300;
    //The amount of time between regeneration i.e. wait 10s before 100 blood is added to your _blood
    private int t_bloodRegenDelay = 10;
    //The amount of time between subtracting blood from Blood
    private float t_bloodLossDelay;

    //Timer used to add Time.DeltaTime to
    private float t_thirsty;
    private float t_hungry;
    private float t_thirst;
    private float t_hunger;
    private float t_blood;
    private float t_bloodRegen;
    private float t_bloodLoss;

    private bool m_initialized = false;

    public float BloodPercent { get { return (_blood / _maxBlood); } }
    public float HungerPercent { get { return (_hunger / _maxHunger); } }
    public float ThirstPercent { get { return (_thirst / _maxThirst); } }
    public float OverallHealth
    {
        get
        {
            return ((BloodPercent + (HungerPercent * 2) + (ThirstPercent * 1.5f)) / 3);
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        BloodRegeneration();
        Bleeding();
        Hunger();
        Thirst();
    }

    //SetupStats sets all the initial values and can only be called once to prevent cheating
    public void SetupStats()
    {
        if(m_initialized)
        {
            Debug.LogError("SetupStats() has already been called once.");
            return;
        }
        _blood = _maxBlood;
        _hunger = _maxHunger;
        _thirst = _maxThirst;

        m_initialized = true;
    }
    public void SetupStats(float maxBlood, float maxHunger, float maxThirst)
    {
        if(m_initialized)
        {
            Debug.LogError("SetupStats() has already been called once from the local machine.");
            Console.LogError("SetupStats() has already been called once from the local machine.");
            return;
        }

        _maxBlood = maxBlood;
        _maxHunger = maxHunger;
        _maxThirst = maxThirst;
        SetupStats();
    }
    public void SetupStats(float maxBlood, float maxHunger, float maxThirst, int thirstDelay, int hungerDelay)
    {
        t_thirstDelay = thirstDelay;
        t_hungerDelay = hungerDelay;
        SetupStats(maxBlood, maxHunger, maxThirst);
    }
    
    public void Damage(float amount)
    {
        if(amount >= _maxBlood)
        {
            Console.LogWarning("Failed To Damage " + transform + " Because the Amount was Greater Than 'MaxBlood'");
            return;
        }

        _blood -= amount;

        if((amount / _maxBlood) >= 0.15f)
        {
            _bleeding = true;
            //Eventually make it so blood loss delay is effected by the amount of damage youve take i.e. more damge decreases the delay
            //t_bloodLossDelay = t_bloodLossDelay * ((amount / (0.5f * _maxBlood) * (0.5f) * _maxBlood));
        }

        if(_blood <= (0.05f * _maxBlood))
        {
            Die("Blood Loss");
        }

        //Reset Blood Regeneration when you take damage
        t_bloodRegen = 0;
    }

    public void Die()
    {
        Die("No Reason");
    }
    public void Die(string reason)
    {
        Console.Log("You Have Died Due To: " + reason + "!");

        //Implement Death Shit here
    }

    //Survival Functions
    private void BloodRegeneration()
    {
        if (t_bloodRegen < t_bloodDelay)
        {
            t_bloodRegen += Time.deltaTime;
            return;
        }

        if(t_blood < t_bloodRegenDelay)
        {
            t_blood += Time.deltaTime;
            return;
        }
        else
        {
            _blood += 50;
            t_blood = 0;
            return;
        }
    }
    private void Bleeding()
    {
        if (!_bleeding)
            return;

        if (t_bloodLoss < t_bloodLossDelay)
        {
            t_bloodLoss += Time.deltaTime;
            return;
        }
        else
        {
            _blood -= 25;
            t_bloodLoss = 0;
            return;
        }
    }
    private void Hunger()
    {
        if(t_hungry < t_hungerDelay)
        {
            t_hungry += Time.deltaTime;
            return;
        }

        if(t_hunger < t_hungerDep)
        {
            t_hunger += Time.deltaTime;
            return;
        }
        else
        {
            _hunger -= 25;
        }
    }
    private void Thirst()
    {
        if (t_thirsty < t_thirstDelay)
        {
            t_thirsty += Time.deltaTime;
            return;
        }

        if (t_thirst < t_thirstDep)
        {
            t_thirst += Time.deltaTime;
            return;
        }
        else
        {
            _thirst -= 25;
        }
    }
}
