using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField userField;
    public InputField passField;

    public Button submitButton;

    public void CallLogin()
    {
        StartCoroutine(LogIn());
    }

    IEnumerator LogIn()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", userField.text);
        form.AddField("password", userField.text);
        WWW www = new WWW("http://localhost/sqlconnect/login.php", form);
        yield return www;
        if(www.text[0] == '0')
        {
            DBManager.username = userField.text;
            DBManager.psych = int.Parse(www.text.Split('\t')[1]);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("User Login Failed. Error #" + www.text);
        }

    }

    public void VerifyInput()
    {
        submitButton.interactable = (userField.text.Length >= 5 && passField.text.Length >= 8);
    }
}
