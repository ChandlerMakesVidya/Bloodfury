using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Name: Player Controller
/// Description: Handles player control of player character.
/// Date Created: 11/01/22
/// Date Updated: 11/25/22
/// </summary>
/// 

/// todo:
/// dashing away from slopes: keep player on slope
/// sliding down steep slopes
/// prevent standing from crouch if terrain does not permit

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Base movement speed of the player.")]
    [SerializeField] float moveSpeed = 10f;

    [Tooltip("Movement speed of the player while dodging.")]
    [SerializeField] float dashSpeed = 30f;

    [Tooltip("Movement speed of the player while crouching.")]
    [SerializeField] float crouchSpeed = 4f;

    [Tooltip("Acceleration with which the player can change their direction in midair.")]
    [SerializeField] float airAcceleration = 20f;

    [Tooltip("Amount by which the player's max speed increases when slide jumping.")]
    [SerializeField] float speedGain = 3f;

    [Tooltip("Rate at which max speed drops back to base move speed.")]
    [SerializeField] float maxSpeedDecay = 20f;

    [Tooltip("Height gained when jumping.")]
    [SerializeField] float jumpHeight = 1.5f;

    //float jumpGrace; //HARDCODED; amount of time after a jump we ignore the ground check
    float jumpQueue; //HARDCODED; how long a jump can be queued for after pressing the jump input

    [Tooltip("The player's height while crouching")]
    [SerializeField] float crouchHeight = 0.75f;

    [Tooltip("Time it takes to transition between standing and crouching.")]
    [SerializeField] float crouchTime = 0.2f;

    float slideJumpTimer;

    [Tooltip("Layers that the ground check responds to.")]
    [SerializeField] LayerMask groundLayerMask;

    float standHeight, camCenterMultiplier = 0.9375f, height, center;
    bool crouching;

    float maxSpeed, prevMaxSpeed, dodgeCooldown = 0.5f, timeToNextDodge, dodgeTime = 0.1f, dodgeStateTime;
    bool justAirborne, grounded, validGround;
    Vector3 wishDir, velocity, prevVelocity, gravVelocity, groundNormal;

    float hori, vert;

    CharacterController charControl;
    Transform head;

    private void Awake()
    {
        charControl = GetComponent<CharacterController>();
        head = GetComponentInChildren<PlayerCamera>().transform;
    }

    private void Start()
    {
        //lock and hide cursor in game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        crouching = false;

        maxSpeed = moveSpeed;
        prevMaxSpeed = maxSpeed;
        timeToNextDodge = 0f;
        standHeight = charControl.height;
        slideJumpTimer = crouchTime;
    }

    private void Update()
    {
        if (Input.GetButtonDown("SlowMo"))
            if (Time.timeScale == 1f) Time.timeScale = 0.5f; else Time.timeScale = 1f;

        hori = Input.GetAxisRaw("Horizontal");
        vert = Input.GetAxisRaw("Vertical");

        if (slideJumpTimer > 0f) slideJumpTimer -= Time.deltaTime;

        //crouching = Input.GetButton("Crouch") ? true : false;
        if (Input.GetButtonDown("Crouch"))
        {
            slideJumpTimer = crouchTime;
        } else if (Input.GetButtonUp("Crouch"))
        {
            slideJumpTimer = 0f;
        }

        if (Input.GetButtonDown("Jump")) jumpQueue = 0.5f;

        //grounded = Physics.CheckSphere(transform.position, 0.25f, groundLayerMask, QueryTriggerInteraction.Ignore);
        //(charControl.collisionFlags & CollisionFlags.Below) != 0 //alt groundcheck
        if (charControl.isGrounded & validGround) grounded = true;
        else grounded = false;
        if (!grounded) groundNormal = Vector3.up;

        //desired direction of movement
        wishDir = Vector3.ClampMagnitude(transform.forward * vert + transform.right * hori, 1.0f);
        wishDir = Vector3.ProjectOnPlane(wishDir, groundNormal);

        //crouching height
        //height = crouching ? crouchHeight : standHeight;
        if (Input.GetButton("Crouch"))
        {
            height = crouchHeight;
            crouching = true;
        }
        else
        {
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.up, standHeight - 0.5f))
            {
                height = standHeight;
                crouching = false;
            }
        }

        if (Time.time >= dodgeStateTime)
        {
            prevMaxSpeed = maxSpeed;
            maxSpeed = velocity.magnitude > moveSpeed ? velocity.magnitude : moveSpeed;
            if (prevMaxSpeed > maxSpeed)
            {
                maxSpeed = Mathf.Lerp(prevMaxSpeed, maxSpeed, maxSpeedDecay * Time.deltaTime);
            }

            prevVelocity = new Vector3(charControl.velocity.x, 0f, charControl.velocity.z);
            if (!grounded /*|| jumpGrace > 0f*/)
            {
                //uncomment this to disable dash jumping
                //if (justDodged) velocity = velocity.normalized * moveSpeed;

                velocity = prevVelocity;
                //velocity.y = 0f; //fixes eternal jumping bug
                velocity += wishDir * airAcceleration * Time.deltaTime;
                if (!justAirborne & gravVelocity.y < 0) gravVelocity = Vector3.zero;
                gravVelocity += Physics.gravity * Time.deltaTime;
                justAirborne = true;
                //if (jumpGrace > 0f) jumpGrace -= Time.deltaTime;
            }
            else
            {
                if (!crouching && Time.time >= timeToNextDodge && Input.GetButtonDown("Dodge"))
                {
                    maxSpeed = dashSpeed;
                    velocity = (wishDir.magnitude > float.Epsilon ? wishDir : Vector3.ProjectOnPlane(transform.forward, groundNormal)) * dashSpeed;
                    timeToNextDodge = Time.time + dodgeCooldown;
                    dodgeStateTime = Time.time + dodgeTime;
                }
                else if(!justAirborne)
                {
                    if (crouching)
                    {
                        velocity = Vector3.Lerp(prevVelocity, wishDir * crouchSpeed,
                            (prevVelocity.magnitude > crouchSpeed ? 1f : 12f) * Time.deltaTime);
                    } else velocity = Vector3.Lerp(prevVelocity, wishDir * moveSpeed, 20f * Time.deltaTime);
                }
                justAirborne = false;

                gravVelocity = Physics.gravity;
                if (jumpQueue > 0f)
                {
                    gravVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                    //jumpGrace = 0.1f;
                    if (crouching)
                    {
                        if (slideJumpTimer > 0f)
                        {
                            maxSpeed += speedGain;
                            velocity = (wishDir.magnitude > float.Epsilon ? wishDir * maxSpeed : velocity.normalized * velocity.magnitude);
                            //velocity = velocity.normalized * maxSpeed;
                            slideJumpTimer = 0f;
                        }
                    }
                    jumpQueue = 0f;
                    //maxSpeed += speedGain;
                }
            }
        }
        else
        {
            gravVelocity = Vector3.zero;
            height = standHeight;
        }

        jumpQueue -= Time.deltaTime;

        center = height / 2;

        if(charControl.height != height)
        {
            charControl.height = Mathf.Lerp(charControl.height, height, crouchTime * 60 * Time.deltaTime);
            charControl.center = Vector3.Lerp(charControl.center, new Vector3(0, center, 0), crouchTime * 60 * Time.deltaTime);
            head.localPosition = new Vector3(0f, charControl.height * camCenterMultiplier, 0f);
        }

        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        //print(velocity.magnitude);

        charControl.Move((velocity + gravVelocity) * Time.deltaTime);
        //print(charControl.velocity.magnitude);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((groundLayerMask.value & (1 << hit.gameObject.layer)) > 0) & Vector3.Angle(hit.normal, Vector3.up) <= charControl.slopeLimit + 0.1f)
        {
            groundNormal = hit.normal;
            validGround = true;
        }
        else
        {
            groundNormal = Vector3.up;
            validGround = false;
        }


    }
}
