using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestruction : MonoBehaviour {

    [Range(1f, 100f)]
    public float timeDelay;

    private void Awake()
    {
        Destroy(gameObject, timeDelay);
    }
    
}
