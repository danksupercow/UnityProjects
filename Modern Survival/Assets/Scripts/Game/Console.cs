using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console instance;
    public GameObject consoleLogPrefab;
    public Color errorColor;

    public Transform consoleTransform;

    private void Awake()
    {
        instance = this;
        Toggle();
    }

    public static void Log()
    {
        if (instance == null)
            return;

        Text t = Instantiate(instance.consoleLogPrefab, instance.transform).GetComponent<Text>();
        t.text = "-";
    }
    public static void Log(object input)
    {
        if (instance == null)
            return;

        Text t = Instantiate(instance.consoleLogPrefab, instance.transform).GetComponent<Text>();
        t.text = " - " + input.ToString();
    }
    public static void Log(object input, Color color)
    {
        if (instance == null)
            return;

        Text t = Instantiate(instance.consoleLogPrefab, instance.transform).GetComponent<Text>();
        t.text = " - " + input.ToString();
        t.color = color;
    }
    public static void LogError(object input)
    {
        if (instance == null)
            return;

        Text t = Instantiate(instance.consoleLogPrefab, instance.transform).GetComponent<Text>();
        t.text = " - " + input.ToString();
        t.color = instance.errorColor;
    }

    public static void Toggle()
    {
        if (instance == null)
            return;

        instance.consoleTransform.gameObject.SetActive(!instance.consoleTransform.gameObject.activeSelf);
    }
}
