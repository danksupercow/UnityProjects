using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : WeaponBase
{
    [Header("Assault Rifle Variables: ")]
    public int hipSpreadAngle;
    public int aimSpreadAngle;

    protected override void PrimaryFire()
    {
        RaycastHit hit;
        Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
        Quaternion ranRot = Random.rotation;

        if(!isAiming)
            fireRotation = Quaternion.RotateTowards(fireRotation, ranRot, Random.Range(0f, hipSpreadAngle));
        else
            fireRotation = Quaternion.RotateTowards(fireRotation, ranRot, Random.Range(0f, aimSpreadAngle));

        audioSource.PlayOneShot(primaryFireSound);
        muzzleFlash.Play(true);
        if (Physics.Raycast(muzzle.position, fireRotation * Vector3.forward, out hit, maxRange, layerMask))
        {
            DamageHandler dh = hit.transform.GetComponent<DamageHandler>();
            if (dh != null)
            {
                dh.Damage(damage, transform);
            }
            else
            {
                Stats s = hit.transform.GetComponent<Stats>();
                if (s != null)
                {
                    s.Damage(damage);
                }
            }

            Transform t = Instantiate(Game.instance.GetImpactFromTag(hit.transform.tag)).transform;
            t.position = hit.point;
            t.LookAt(transform);
            t.SetParent(hit.transform);
        }
    }
}
