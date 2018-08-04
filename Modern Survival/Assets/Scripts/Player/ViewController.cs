using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ViewController : MonoBehaviour {
    public static ViewController instance;

    public Transform hand;
    private Camera cam;
    public static Ray playerRay;
    private WeaponBase currentWeapon;
    private ExplosiveBase currentExplosive;
    public int connectionID;

    Stats localStats;

    public Stats stats { get { return localStats; } }

    private void Start()
    {
        localStats = GetComponent<Stats>();

        if (connectionID != NetworkManager.connectionID)
            return;

        instance = this;
        cam = GetComponentInChildren<Camera>();
        if(hand.childCount > 0)
        {
            currentWeapon = hand.GetComponentInChildren<WeaponBase>();
            if(currentWeapon != null)
                currentWeapon.CallStart();
            currentExplosive = hand.GetComponentInChildren<ExplosiveBase>();
            if (currentExplosive != null)
                currentExplosive.CallStart();
        }

        if(currentWeapon!=null)
        {
            currentWeapon.owner = this;
        }
    }

    private void Update()
    {
        if (connectionID != NetworkManager.connectionID)
            return;

        playerRay = cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if(currentWeapon == null && hand.childCount > 0)
        {
            currentWeapon = hand.GetComponentInChildren<WeaponBase>();
            if(currentWeapon != null)
            {
                currentWeapon.CallStart();
            }
        }

        if(currentExplosive == null && hand.childCount > 0)
        {
            currentExplosive = hand.GetComponentInChildren<ExplosiveBase>();
            if(currentExplosive != null)
            {
                currentExplosive.CallStart();
            }
        }

        if (currentWeapon != null && currentWeapon.gameObject.activeSelf && !PlayerController.instance.toggleEscMenu)
        {
            currentWeapon.CallUpdate();
        }

        if (currentExplosive != null && currentExplosive.gameObject.activeSelf && !PlayerController.instance.toggleEscMenu)
            currentExplosive.CallUpdate();
    }
}
