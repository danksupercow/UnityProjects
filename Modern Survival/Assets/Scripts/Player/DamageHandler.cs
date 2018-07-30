using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public Stats localStats;
    public float damageMultiplier;

    public void Damage(float amount, Transform sender)
    {
        //localStats.CmdDamage(amount * damageMultiplier);
    }
}
