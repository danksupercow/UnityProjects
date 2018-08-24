using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float smoothSpeed = 0.125f; // between 0 & 1
    public Vector3 offset;

    private void FixedUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        transform.position = smoothedPos;
    }

}
