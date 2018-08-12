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

            Projectile p = Instantiate(projectile.prefab, muzzle.position, muzzle.rotation).GetComponent<Projectile>();
            p.damage = damage;
            p.hitCallback = HitCallback;
            p.rigidbody.AddForce(fireRotation.eulerAngles * fireForce, ForceMode.Impulse);

        }
    }
}
