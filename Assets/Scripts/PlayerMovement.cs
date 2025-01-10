using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float lateralSpeed, accelerationSpeed, turningSpeed;
    private float forwardMomentum = 0f;
    private float turnMomentum = 0f;
    private int direction = 1;

    private CapsuleCollider2D monColl;
    private Rigidbody2D rb;
    private Collider2D[] colls;
    private SpriteRenderer skin;
    private Animator anim;
    private Hook hookScript;

    [HideInInspector] public bool grounded;
    public float decalage;
    private bool hooking;
    private bool grappled;

    [Serializable] public class HeroJumpSettings
    {
        public float jumpSpeed;
        public float jumpMinDuration;
        public float jumpMaxDuration;
    }

    [SerializeField] private HeroJumpSettings jumpSettings;
    private float _jumpTimer;

    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling
    }

    private JumpState _jumpState;

    private bool isJumpUpgraded = false;
    private bool doubleJump = false;

    private DistanceJoint2D joint;
    [SerializeField] private LineRenderer rope1;
    public bool key;

    public GameObject hud;

    // Start is called before the first frame update
    void Start()
    {
        hookScript = GetComponent<Hook>();
        rb = GetComponent<Rigidbody2D>();
        monColl = GetComponent<CapsuleCollider2D>();
        skin = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCheck();
        AccelerationInertia();
        StoppingInertia();
        TurningInertia();
        CancelInertiaOnGroundReception();
        JumpCheck();
        GroundCheck();
        OnMovingPlatformCheck();
        RotateCheck();
        HookingCheck();
        GrappledCheck();
        AnimCheck();
    }

    void MoveCheck() //Main movement function : changes the player's velocity based on the speed values and inertia values
    {
        if (!joint.enabled && !rope1.enabled)
        {
            rb.velocity = new Vector2
                (
                Input.GetAxis("Horizontal") * lateralSpeed 
                + forwardMomentum * accelerationSpeed * Input.GetAxis("Horizontal")
                + turnMomentum * turningSpeed,

                rb.velocity.y
                );
        }
    } 

    void JumpStart() //Enables the jump if all the requirements are met, and checks if the double jump is unlocked
    {
        if (_jumpState == JumpState.NotJumping && grounded)
        {
            _jumpState = JumpState.JumpImpulsion;
            _jumpTimer = 0f;
        }
        else if (_jumpState != JumpState.NotJumping && doubleJump)
        {
            _jumpState = JumpState.JumpImpulsion;
            _jumpTimer = 0f;
            doubleJump = false;
        }
    }

    private bool IsJumping => _jumpState != JumpState.NotJumping;
    private bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    private bool IsJumpMinDurationReached => _jumpTimer >= jumpSettings.jumpMinDuration;

    void StopJumpImpulsion() //Changes the jump state to the part where it falls
    {
        _jumpState = JumpState.Falling;
    }

    void UpdateJumpStateImpulsion() //Changes the vertical velocity if the player is jumping, and calls the previous function upon reaching the maximal jump time
    {
        _jumpTimer += Time.deltaTime;
        if (_jumpTimer < jumpSettings.jumpMaxDuration) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSettings.jumpSpeed);
        }
        else
        {
            StopJumpImpulsion();
        }
    }

    void UpdateJumpStateFalling() //If the ground is met, changes the jump's state to enable another jump
    {
        if (grounded)
        {
            _jumpState = JumpState.NotJumping;
            if (isJumpUpgraded) { doubleJump = true; }
        }
    }

    void JumpUpdate() //Calls the previous functions depending on the jump's state
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                UpdateJumpStateImpulsion();
                break;

            case JumpState.Falling:
                UpdateJumpStateFalling();
                break;
        }
    }

    void JumpCheck() //Mains jump function : calls previous jump-related functions, either when inputting a jump, or when the jump is active
    {
        if (IsJumping)
        {
            JumpUpdate();
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpStart();
        }

        if (IsJumpImpulsing)
        {
            if(!Input.GetButton("Jump") && IsJumpMinDurationReached)
            {
                StopJumpImpulsion();
            }
        }
    }

    public void UpgradeJump() //Enables the double jump if it has been bought in the game's upgrade shop
    {
        if (hud.GetComponent<MoneyCount>().money >= 200)
        {
            isJumpUpgraded = true;
            hud.SendMessage("MoneyDown", 200);
        }
    }

    void AccelerationInertia() //Adds a first, small boost to the player's movement if he is moving in either lateral direction
    {
        if (!joint.enabled && !rope1.enabled)
        {
            if (Input.GetAxis("Horizontal") != 0 && grounded)
            {
                if (forwardMomentum <= 1f)
                {
                    forwardMomentum += Time.deltaTime * 2;
                }
            }
        }
    }

    void StoppingInertia() //Manages said first inertia when stopping lateral movement
    {
        if (!joint.enabled && !rope1.enabled)
        {
            if (Input.GetAxis("Horizontal") == 0)
            {
                if (forwardMomentum > 0f)
                {
                    forwardMomentum -= Time.deltaTime;
                }
                else
                {
                    forwardMomentum = 0f;
                }
            }
        }            
    }

    void TurningInertia() //Adds a second, stronger inertia which direction is kept upon changing direction, allowing for more physically realistic turning 
    {
        if (!joint.enabled && !rope1.enabled)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                if (turnMomentum < 1f && direction == 1)
                {
                    turnMomentum += Time.deltaTime * 2;
                }
                else if (turnMomentum > -1f && direction == -1)
                {
                    turnMomentum -= Time.deltaTime * 2;
                }
            }
            else
            {
                if (turnMomentum > 0f)
                {
                    turnMomentum -= Time.deltaTime * 3.5f;
                }
                else if (turnMomentum < -0.2f)
                {
                    turnMomentum += Time.deltaTime * 3.5f;
                }
                else
                {
                    turnMomentum = 0f;
                }
            }
        }
    }

    void CancelInertiaOnGroundReception() //Upon receptioning on the ground after a jump, cuts down the second inertia by a quarter
    {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "playerReception")
        {
            turnMomentum *= 0.75f;
        }
    }

    void GroundCheck() //Checks for any solid ground under the player's feet
    {
        grounded = false;

        colls = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, monColl.bounds.min.y + decalage), monColl.bounds.size.x * 0.4f);

        foreach (Collider2D coll in colls)
        {
            if (coll != monColl && !coll.isTrigger)
            {
                grounded = true;
                break;
            }
        }
    }

    void OnMovingPlatformCheck() //Cancels any movement if the player is activating a moving platform
    {
        if (hookScript.platformCheck && grounded)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void HookingCheck() //Enables an amount of the hook-related animations
    {
        if(hookScript.hazardCheck || hookScript.platformCheck || GameObject.FindGameObjectWithTag("Ouk Ed"))
        {
            hooking = true;
        }
        else
        {
            hooking = false;
        }
    }

    void GrappledCheck() //Enables the rest of the hook-related animations
    {
        if (hookScript.dashCheck || hookScript.swingCheck || hookScript.slowCheck || hookScript.climbCheck)
        {
            grappled = true;
        }
        else
        {
            grappled= false;
        }
    }

    void RotateCheck() //Flips the player's sprite based on its lateral speed, but not if the player is in the air
    {
        if (Input.GetAxis("Horizontal") < 0)
        {
            direction = -1;

            if(rb.velocity.x  < 0 && grounded)
            {
                skin.flipX = true;
            }
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            direction = 1;

            if (rb.velocity.x > 0 && grounded)
            {
                skin.flipX = false;
            }
        }
    }

    void AnimCheck() //Manages the different parameters of the player's animator component
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("grounded", grounded);
        anim.SetBool("grappled", grappled);
        anim.SetBool("hookingAir", hooking && !grounded);
        anim.SetBool("hookingGround", hooking && grounded);
    }

    private void OnDrawGizmos() //Draws editing-related gizmos on the zone that checks the player's grounding
    {
        if (monColl == null)
        {
            monColl = GetComponent<CapsuleCollider2D>();
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, monColl.bounds.min.y + decalage), monColl.bounds.size.x * 0.4f);
    }
}
