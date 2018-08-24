using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform follower;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, follower.position, 50 * Time.deltaTime);
    }
}
