using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private Stats localStats;
    public float damageMultiplier;

    private void Awake()
    {
        localStats = GetComponentInParent<Stats>();
    }

    public void Damage(float amount)
    {
        try
        {
            localStats.Damage((amount * damageMultiplier));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
