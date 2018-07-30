using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Base Weapon Variables: ")]
    public float damage = 1.0f;
    public float maxRange;
    public int maxAmmo = 0; // 0 = infinite ammo
    public float fireDelay = 0.1f;
    public string primaryFire = "Fire1";
    public string secondaryFire = "Fire2";
    public LayerMask layerMask = -1;
    public bool isAutomatic = false;
    public bool isAiming = false;
    public Vector3 aimPosition;
    public Vector3 hipPosition;
    protected Transform muzzle;
    protected ParticleSystem muzzleFlash;
    [HideInInspector]
    public ViewController owner;

    public AudioClip primaryFireSound;
    public AudioClip reloadSound;
    protected AudioSource audioSource;

    private bool readyToFire = true;
    private int currentAmmo;

    protected abstract void PrimaryFire();

    public void CallStart()
    {
        audioSource = GetComponent<AudioSource>();
        muzzle = transform.Find("muzzle");
        muzzleFlash = muzzle.GetComponentInChildren<ParticleSystem>();
        currentAmmo = maxAmmo;

        Debug.Log("Weapon " + transform + ": Audio=" + audioSource + " Muzzle=" + muzzle + "Starting Ammo=" + currentAmmo);
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
            if (readyToFire && (currentAmmo > 0 || maxAmmo == 0))
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
        if(isAiming)
        {
            transform.localPosition = aimPosition;
        }
        else
        {
            transform.localPosition = hipPosition;
        }
    }

    private void SetReadyToFire()
    {
        readyToFire = true;
    }
}
