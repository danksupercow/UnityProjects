using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private Stats localStats;
    private PlayerController ply;
    public GameObject bleedingNotification;
    public GameObject deathScreen;
    public Image healthBar;

    private void Start()
    {
        localStats = GetComponent<Stats>();
        ply = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (localStats.enabled == false)
        {
            bleedingNotification.SetActive(false);
            healthBar.gameObject.SetActive(false);
            deathScreen.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else
        {
            bleedingNotification.SetActive(true);
            healthBar.gameObject.SetActive(true);
            deathScreen.SetActive(false);
        }

        //bleedingNotification.SetActive(localStats.isBleeding);
        //healthBar.fillAmount = (localStats.health / Game.instance.maxPlayerHealth);
    }

    public void Respawn()
    {
        transform.position = new Vector3(0.7f, 7.1f, 20f);
        transform.rotation = Quaternion.identity;
        ply.enabled = true;
        localStats.enabled = true;
        //localStats.Reset();
    }
}
