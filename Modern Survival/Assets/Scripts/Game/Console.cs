﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console instance;

    private Text consoleText;
    private string consoleString;

    public Transform consoleTransform;
    public Text inputText;

    private void Awake()
    {
        instance = this;
        consoleText = GetComponent<Text>();
    }

    private void LateUpdate()
    {
        consoleText.text = consoleString;
    }

    public static void Log(object input)
    {
        instance.consoleString += (" - " + input.ToString() + "\n");
    }

    public static void LogError(object input)
    {
        instance.consoleString += (" - [ ERROR ] " + input.ToString() + "\n");
    }

    public static void Toggle()
    {
        if (instance == null)
            return;

        instance.consoleTransform.gameObject.SetActive(!instance.consoleTransform.gameObject.activeSelf);
    }

    public void EndEdit()
    {
        if(Input.GetButtonDown("Submit"))
        {
            RunCommand(inputText.text);
        }
    }

    private void RunCommand(string input)
    {
        if(input.ToLower().StartsWith("connect"))
        {
            string s = input.Replace(' ', ':');
            string[] vars = s.Split(':');
            if(vars.Length != 3)
            {
                LogError("Failed to Run 'connect' command not 3 variables");
                if(vars.Length == 2 && vars[1].ToLower() == "localhost")
                {
                    NetworkManager.Client.Connect();
                }
                return;
            }

            NetworkManager.Client.Connect(vars[1], int.Parse(vars[2]));
        }
        if(input.ToLower().StartsWith("newname"))
        {
            string s = input.Replace(' ', ':');
            string[] vars = s.Split(':');

            NetworkManager.FetchServerData(ClientTCP.CurrentIP, ClientTCP.CurrentPort).Name = vars[1];
        }
    }
}