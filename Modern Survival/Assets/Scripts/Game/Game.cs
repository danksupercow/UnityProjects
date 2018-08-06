using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    public static Game instance;

    public float maxPlayerHealth;
    public float startPlayerHealth;

    [Header("Impact Particle Effects")]
    public GameObject fleshImpact;
    public GameObject dirtImpact;
    public GameObject rockImpact;
    public GameObject metalImpact;
    public GameObject glassImpact;

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
            case "Glass":
                return glassImpact;
            default:
                break;
        }

        return rockImpact;
    }
    public static string GetSlugFromTag(string tag)
    {
        switch (tag)
        {
            case "Living":
                return "flesh_impact";
            case "Player":
                return "flesh_impact";
            case "Terrain":
                return "dirt_impact";
            case "Metal":
                return "metal_impact";
            case "Glass":
                return "glass_impact";
            case "Rock":
                return "rock_impact";
            default:
                break;
        }

        return "rock_impact";
    }


}
