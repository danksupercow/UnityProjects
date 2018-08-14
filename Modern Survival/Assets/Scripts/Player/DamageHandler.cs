using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public Stats localStats;
    public float damageMultiplier;

    public void Damage(float amount)
    {
        try
        {
            //localStats.Damage((amount * damageMultiplier));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
