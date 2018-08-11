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

        /* ###    \/ Bullet Spread( Different When Aiming ) \/    ### */
        if (!isAiming)
            fireRotation = Quaternion.RotateTowards(fireRotation, ranRot, Random.Range(0f, hipSpreadAngle));
        else
            fireRotation = Quaternion.RotateTowards(fireRotation, ranRot, Random.Range(0f, aimSpreadAngle));

        /* ###    \/ Muzzle Flash Stuff \/    ### */
        audioSource.PlayOneShot(primaryFireSound);
        muzzleFlash.Play(true);


        if (Physics.Raycast(muzzle.position, fireRotation * Vector3.forward, out hit, maxRange, layerMask))
        {
            Console.Log("You Hit " + hit.transform.name + " from " + hit.distance + "m.");

            /* ###    \/ Damage && Physics Stuff \/    ### */
            ViewController vc = hit.transform.GetComponent<ViewController>();
            DamageHandler dh = hit.transform.GetComponent<DamageHandler>();
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();

            AddWeaponForceAtPoint(rb, hit.point);

            if (NetworkManager.instance != null && vc != null && dh != null)
            {
                int id = vc.connectionID;
                DealNetworkedDamage(id, (damage * dh.damageMultiplier));
            }
            else if(dh != null)
            {
                dh.Damage(damage);
            }

            /* ###    \/ Hit Impact Effects \/    ### */
            Transform t = Instantiate(Game.instance.GetImpactFromTag(hit.transform.tag)).transform;
            t.position = hit.point;
            t.LookAt(transform);
        }
    }
}
