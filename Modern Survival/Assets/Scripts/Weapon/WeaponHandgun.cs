using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandgun : WeaponBase
{
    protected override void PrimaryFire()
    {
        RaycastHit hit;
        Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
        
        audioSource.PlayOneShot(primaryFireSound);
        muzzleFlash.Play(true);

        if(Physics.Raycast(muzzle.position, fireRotation*Vector3.forward, out hit, maxRange, layerMask))
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
            if(NetworkManager.instance != null)
            {
                ClientTCP.SpawnRegisteredPrefab(Game.GetSlugFromTag(hit.transform.tag), hit.point, Quaternion.identity);
            }
            else
            {
                Transform t = Instantiate(Game.instance.GetImpactFromTag(hit.transform.tag)).transform;
                t.position = hit.point;
                t.LookAt(transform);
            }
        }
    }
}
