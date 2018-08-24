using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text userStatus;

    private void Start()
    {
        if(DBManager.LoggedIn)
        {
            userStatus.text = DBManager.username;
        }
    }

    public void RegistrationScreen()
    {
        SceneManager.LoadScene(1);
    }
    public void LoginScreen()
    {
        SceneManager.LoadScene(2);
    }
}
