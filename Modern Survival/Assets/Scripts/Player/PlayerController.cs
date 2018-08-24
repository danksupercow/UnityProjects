using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    
    public static PlayerController instance;

    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float crouchSpeed = 2f;
    public float walkSpeed = 8f;
    public float runSpeed = 12f;
    public float jumpForce = 220f;

    public LayerMask groundedMask;
    public float groundedThreshold = 0.1f;
    public float fallingDistance = 5f;

    public SyncAnimator animator;
    private float moveSpeed;
    public float groundDistance;

    [HideInInspector]
    public bool toggleEscMenu = false;

    //Player Components
    Rigidbody rb;

    Transform cameraT;
    float verticalClamp;
    float t_grounded;
    
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    Vector3 moveDir;

    public bool grounded;

    private float animMove = 0f;
    public float animationTransition = 5f;
    public float speedTransition = 1.5f;
    
    //Private
    private Camera cam;
    [SerializeField]
    private Camera fpp;
    [SerializeField]
    private Camera tpp;

    private bool b_fpp;

    private PlayerAnimationController animationController;

    public Camera Camera { get { return cam; } }

    private void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();
    }

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
        if (Input.GetButtonDown("ToggleView"))
            b_fpp = !b_fpp;

        if (b_fpp)
        {
            tpp.gameObject.SetActive(false);
            fpp.gameObject.SetActive(true);
            cam = fpp;
        }else
        {
            tpp.gameObject.SetActive(true);
            fpp.gameObject.SetActive(false);
            cam = fpp;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            toggleEscMenu = !toggleEscMenu;
        }

        if (moveDir.z == 1)
            animationController.movingForward = true;
        else
            animationController.movingForward = false;

        if (moveDir.z == -1)
            animationController.movingBackward = true;
        else
            animationController.movingBackward = false;

        if (moveDir.z == -1)
        {
            animMove = 0.5f;
        }

        if (Input.GetButton("Run") && animationController.movingForward && !animationController.crouched)
        {
            animationController.running = true;
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, speedTransition);
            animMove = 1.5f;
        }
        else if(animationController.movingForward)
        {
            animationController.running = false;
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, speedTransition);
        }

        if (animationController.movingBackward)
        {
            animationController.running = false;
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, speedTransition * Time.deltaTime);
            animMove = 1f;
        }

        if (!animationController.movingForward)
        {
            animationController.running = false;
            moveSpeed = Mathf.Lerp(moveSpeed, 0f, speedTransition * Time.deltaTime);
            animMove = 0f;
        }

        if(animationController.movingForward && !animationController.running)
        {
            animMove = 1f;
        }

        if(animationController.movingBackward)
        {
            animMove = 0.5f;
        }

        if (animationController.crouched && animationController.movingForward && !animationController.running)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, speedTransition * Time.deltaTime);
            animMove = 1;
        }

        if (!animationController.movingForward && !animationController.running)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, speedTransition * Time.deltaTime);
            animMove = 0f;
        }
        
        if (groundDistance <= groundedThreshold)
        {
            grounded = true;
            animationController.falling = false;
        }
        else if(groundDistance >= fallingDistance && t_grounded >= 0.25f)
        {
            animationController.falling = true;
            grounded = false;
        }
        else
        {
            animationController.falling = false;
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
            {
                animationController.Jump();
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, groundedMask))
        {
            groundDistance = hit.distance;
        }



        animationController.grounded = grounded;
        animationController.animMove = animMove;
        animationController.animationTransition = animationTransition;

        Move();

    }

    private void Move()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
        verticalClamp += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalClamp = Mathf.Clamp(verticalClamp, -60, 60);
        cam.transform.localEulerAngles = Vector3.left * verticalClamp;

        if (grounded)
            t_grounded = 0f;

        if (!grounded)
            t_grounded += Time.deltaTime;

        if(t_grounded >= 0.25f)
        {
            return;
        }
        
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDir * moveSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        //transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (toggleEscMenu)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
