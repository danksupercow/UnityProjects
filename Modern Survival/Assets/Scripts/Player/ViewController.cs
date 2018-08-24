using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ViewController : MonoBehaviour {
    public static ViewController instance;

    public Transform hand;
    public Camera cam;
    public static Ray playerRay;
    private WeaponBase currentWeapon;
    private ExplosiveBase currentExplosive;
    public int connectionID;
    public SyncAnimator animator;

    Stats localStats;

    public Stats stats { get { return localStats; } }

    private void Start()
    {
        localStats = GetComponent<Stats>();
        animator = GetComponent<SyncAnimator>();
        
        instance = this;
        if(hand.childCount > 0)
        {
            currentWeapon = hand.GetComponentInChildren<WeaponBase>();
            if(currentWeapon != null)
                currentWeapon.CallStart();
            currentExplosive = hand.GetComponentInChildren<ExplosiveBase>();
            if (currentExplosive != null)
                currentExplosive.CallStart();
        }

        Console.Toggle();
    }

    private void Update()
    {
        CheckInput();

        playerRay = cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        if(currentWeapon != null && currentWeapon.gameObject.activeSelf == false)
        {
            currentWeapon = null;
        }

        if (currentWeapon == null && hand.childCount > 0)
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

        if(Input.GetButton("Fire2"))
        {
            animator.SetBool("AimHandgun", true);
        }else
        {
            animator.SetBool("AimHandgun", false);
        }
    }

    void CheckInput()
    {
        if(Input.GetButtonDown("Console"))
        {
            Console.Toggle();
        }

        if (Input.GetKeyDown(KeyCode.Q) && NetworkManager.instance != null && NetworkManager.Connected)
        {
            ClientTCP.SpawnRegisteredPrefab("billy_the_kid", new Vector3(5, 1, 5), Quaternion.identity);
        }
    }
}
