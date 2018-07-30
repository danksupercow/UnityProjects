using UnityEngine;
using System.Collections;

[System.Serializable]
public class Range
{
    [SerializeField]
    private float _min;
    [SerializeField]
    private float _max;

    public float Min { get { return _min; } }
    public float Max { get { return _max; } }

    public Range(float min, float max)
    {
        _min = min;
        _max = max;
    }

}
