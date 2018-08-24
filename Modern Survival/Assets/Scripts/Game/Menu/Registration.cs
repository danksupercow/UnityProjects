using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Registration : MonoBehaviour
{
    public InputField userField;
    public InputField passField;

    public Button submitButton;
    
    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", userField.text);
        form.AddField("password", userField.text);
        WWW www = new WWW("http://localhost/sqlconnect/register.php", form);
        yield return www;
        if(www.text == "0")
        {
            Debug.Log("User Created Successfully!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("User Creation Failed! Error #" + www.text);
        }
    }

    public void VerifyInput()
    {
        submitButton.interactable = (userField.text.Length >= 5 && passField.text.Length >= 8);
    }
}
