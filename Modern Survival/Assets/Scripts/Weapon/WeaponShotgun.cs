using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponShotgun : WeaponBase
{
    [Header("Shotgun Variables: ")]
    public int pelletCount = 8;
    [Range(10, 45)]
    public float spreadAngle = 10;
    public AudioClip pumpSound;

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

            if(Physics.Raycast(muzzle.position, fireRotation * Vector3.forward, out hit, maxRaycastDist))
            {
                SpawnImpactEffect(hit.transform.tag, hit.point);
                continue;
            }
            else
            {
                Projectile p = Instantiate(projectile.prefab, muzzle.position, muzzle.rotation).GetComponent<Projectile>();
                p.damage = damage;
                p.hitCallback = HitCallback;
                p.rigidbody.AddForce((fireRotation * Vector3.forward) * fireForce, ForceMode.Impulse);
            }

        }

        audioSource.PlayOneShot(pumpSound);
    }
}
