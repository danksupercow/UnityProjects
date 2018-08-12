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

        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, maxRaycastDist))
        {
            /* ###    \/ Damage && Physics Stuff \/    ### */
            ViewController vc = hit.transform.GetComponent<ViewController>();
            DamageHandler dh = hit.transform.GetComponent<DamageHandler>();
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();

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
            if (NetworkManager.instance != null)
            {
                ClientTCP.SpawnRegisteredPrefab(Game.GetSlugFromTag(hit.transform.tag), hit.point, Quaternion.identity);
            }
            else
            {
                SpawnImpactEffect(hit.transform.tag, hit.point);
            }

            return;
        }

        Projectile p = Instantiate(projectile.prefab, muzzle.position, muzzle.rotation).GetComponent<Projectile>();
        p.damage = damage;
        p.hitCallback = HitCallback;
        p.rigidbody.AddForce(transform.forward * fireForce, ForceMode.Impulse);
    }
}
