using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Base Projectile Weapon Variables: ")]
    public Projectile projectile;
    public float fireForce = 100f;
    public float maxRaycastDist = 20f;
    public float damage = 50f;
    public int maxAmmo = 0; // 0 = infinite ammo

    [Header("Fire Info: ")]
    public float fireDelay = 0.1f;
    public float hitForce = 100;
    public bool isAutomatic = false;
    public string primaryFire = "Fire1";
    public string secondaryFire = "Fire2";
    
    [Header("Aim Info: ")]
    public float aimSpeed = 5;
    public Vector3 aimPosition;
    public Vector3 hipPosition;

    protected Transform muzzle;
    protected ParticleSystem muzzleFlash;

    [Header("Audio Info: ")]
    public AudioClip primaryFireSound;
    public AudioClip reloadSound;
    protected AudioSource audioSource;

    private bool readyToFire = true;
    private int currentAmmo;

    protected bool canFire = true;
    protected bool isAiming = false;

    protected abstract void PrimaryFire();

    protected virtual void HitCallback(Transform t, Vector3 pos)
    {
        SpawnImpactEffect(t.tag, pos);
        Rigidbody rb = t.GetComponent<Rigidbody>();
        AddWeaponForceAtPoint(rb, pos);
    }

    public void CallStart()
    {
        audioSource = GetComponent<AudioSource>();
        muzzle = transform.Find("muzzle");
        muzzleFlash = muzzle.GetComponentInChildren<ParticleSystem>();
        currentAmmo = maxAmmo;
    }

    public void CallUpdate()
    {
        CheckInput();
        AimDownSights();
    }

    protected virtual void CheckInput()
    {
        bool primaryPressed;

        isAiming = Input.GetButton(secondaryFire);

        if(Input.GetButtonDown("Reload"))
        {
            StartCoroutine(Reload());
        }

        if (isAutomatic)
        {
            primaryPressed = Input.GetButton(primaryFire);
        }
        else
        {
            primaryPressed = Input.GetButtonDown(primaryFire);
        }

        if (primaryPressed)
        {
            if (readyToFire && canFire && (currentAmmo > 0 || maxAmmo == 0))
            {
                PrimaryFire();
                readyToFire = false;
                currentAmmo--;
                Invoke("SetReadyToFire", fireDelay);
            }
        }
    }

    protected virtual void AimDownSights()
    {
        if (isAiming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aimSpeed);

        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipPosition, Time.deltaTime * aimSpeed);
        }
    }

    private void SetReadyToFire()
    {
        readyToFire = true;
    }

    protected void DealNetworkedDamage(int connectionID, float damage)
    {
        ClientTCP.SendDamage(connectionID, damage);
    }

    protected virtual void AddWeaponForceAtPoint(Rigidbody rb, Vector3 point)
    {
        if (rb == null)
        {
            return;
        }

        Vector3 dir = point - transform.position;
        rb.AddForceAtPosition(dir.normalized * hitForce, point, ForceMode.Impulse);
    }

    protected virtual void SpawnImpactEffect(string tag, Vector3 pos)
    {
        ObjectPooler.instance.SpawnFromPool(Game.GetSlugFromTag(tag), pos, Quaternion.identity);
    }

    protected virtual IEnumerator Reload()
    {
        if (maxAmmo > 0)
        {
            canFire = false;
            audioSource.PlayOneShot(reloadSound);
            yield return new WaitForSeconds(reloadSound.length + 0.1f);
            currentAmmo = maxAmmo;
            canFire = true;
        }

    }
}
