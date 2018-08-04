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
            int id = hit.transform.GetComponent<ViewController>().connectionID;

            DealNetworkedDamage(id, damage);

            if(Game.instance != null)
            {
                Transform t = Instantiate(Game.instance.GetImpactFromTag(hit.transform.tag)).transform;
                t.position = hit.point;
                t.LookAt(transform);
                t.SetParent(hit.transform);
            }
        }
    }
}
