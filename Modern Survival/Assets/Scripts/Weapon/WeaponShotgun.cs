using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponShotgun : WeaponBase
{
    [Header("Shotgun Variables: ")]
    public int pelletCount = 8;
    public float spreadAngle = 10;

    protected override void PrimaryFire()
    {
        audioSource.PlayOneShot(primaryFireSound);
        muzzleFlash.Play(true);

        for (int i = 0; i < pelletCount; i++)
        {
            RaycastHit hit;
            Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
            Quaternion ranRot = Random.rotation;

            fireRotation = Quaternion.RotateTowards(fireRotation, ranRot, Random.Range(0f, spreadAngle));

            if (Physics.Raycast(transform.position, fireRotation*Vector3.forward, out hit, maxRange, layerMask))
            {
                ViewController vc = hit.transform.GetComponent<ViewController>();
                DamageHandler dh = hit.transform.GetComponent<DamageHandler>();

                if (NetworkManager.instance != null && vc != null && dh != null)
                {
                    int id = vc.connectionID;
                    DealNetworkedDamage(id, (damage * dh.damageMultiplier));
                }
                else if (dh != null)
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
}
