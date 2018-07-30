using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed = 8f;
    public float runSpeed = 12f;
    public float jumpForce = 220f;
    private float moveSpeed;
    private float groundDistance;

    public bool toggleEscMenu = false;

    //Player Components
    Rigidbody rb;

    Transform cameraT;
    float verticalClamp;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    Vector3 lastPosition;

    bool grounded;

    //Private
    private Camera cam;
    public Camera Camera { get { return cam; } }

    private void Start()
    {
        instance = this;

        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            toggleEscMenu = !toggleEscMenu;
        }

        if (Input.GetButton("Run"))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        if(groundDistance <= 1.3f)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        
        if (toggleEscMenu)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            groundDistance = hit.distance;
        }


        Move();

    }

    private void LateUpdate()
    {
        NetSendMove();
    }

    private void Move()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
        verticalClamp += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalClamp = Mathf.Clamp(verticalClamp, -60, 60);
        cam.transform.localEulerAngles = Vector3.left * verticalClamp;

        if (!grounded)
            return;
        
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDir * moveSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

    }

    private void FixedUpdate()
    {
        if (toggleEscMenu)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
    
    private void NetSendMove()
    {

        if(Vector3.Distance(lastPosition, transform.position) >= 0.2f && NetworkManager.instance != null && NetworkManager.instance.Socket != null)
        {
            ClientTCP.SendMovement(transform.position, transform.rotation);
            lastPosition = transform.position;
        }
    }

}
