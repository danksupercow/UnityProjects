using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Game : MonoBehaviour
{
    public static Game instance;
    public float maxPlayerHealth = 500;

    [Header("Impact Particle Effects")]
    public GameObject fleshImpact;
    public GameObject dirtImpact;
    public GameObject rockImpact;
    public GameObject metalImpact;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetImpactFromTag(string tag)
    {
        switch (tag)
        {
            case "Living":
                return fleshImpact;
            case "Player":
                return fleshImpact;
            case "Terrain":
                return dirtImpact;
            case "Metal":
                return metalImpact;
            default:
                break;
        }

        return rockImpact;
    }
}
