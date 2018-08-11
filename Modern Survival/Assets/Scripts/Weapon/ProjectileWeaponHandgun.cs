using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponHandgun : ProjectileWeaponBase
{
    protected override void PrimaryFire()
    {
        RaycastHit hit;

        if(muzzleFlash != null)
            muzzleFlash.Play();
        if (audioSource != null)
            audioSource.PlayOneShot(primaryFireSound);

        if(Physics.Raycast(muzzle.position, muzzle.forward, out hit, maxRaycastDist))
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
        p.hitCallback = HitCallback;
        p.rigidbody.AddForce(transform.forward * fireForce, ForceMode.Impulse);
    }

    protected override void HitCallback(Transform t, Vector3 pos)
    {
        SpawnImpactEffect(t.tag, pos);
        Rigidbody rb = t.GetComponent<Rigidbody>();
    }
}
