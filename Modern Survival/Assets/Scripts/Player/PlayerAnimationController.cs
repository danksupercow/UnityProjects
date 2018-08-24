using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SyncAnimator))]
public class PlayerAnimationController : MonoBehaviour {
    
    private SyncAnimator animator;

    public bool crouched;
    public bool movingForward;
    public bool movingBackward;
    public bool running;
    public bool grounded;

    public bool falling;

    public float animMove;
    public float animationTransition;

	void Awake ()
    {
        animator = GetComponent<SyncAnimator>();
	}
	
	void Update () {
        StateCheck();
	}

    private void StateCheck()
    {
        #region Leaning
        if (Input.GetButton("Lean Left"))
            animator.SetBool("LeanLeft", true);
        else
            animator.SetBool("LeanLeft", false);

        if (Input.GetButton("Lean Right"))
            animator.SetBool("LeanRight", true);
        else
            animator.SetBool("LeanRight", false);
        #endregion

        #region Attacking
        if (Input.GetButtonDown("Fire1") && !crouched && !falling && !running)
        {
            animator.SetTrigger("Punch");
        }
        #endregion

        #region Movement
        crouched = Input.GetButton("Crouch");
        animator.SetFloat("speedPercent", animMove, animationTransition, Time.deltaTime);
        animator.SetBool("isCrouched", crouched);
        animator.SetBool("isFalling", falling);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Running", running);
        #endregion
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
}
