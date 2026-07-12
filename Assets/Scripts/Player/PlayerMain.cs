using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    PlayerIk ikPlayer;
    HpSystem hpSys;
    ControllerSettings control;
    Camera cam;
    [HideInInspector]
    public CharacterController controller;
    public LayerMask groundMask;
    //PlayerCamera plyrCam;

    [Header("Check Vars (Read-only)")]
    public bool isPlayerGrounded;
    public bool isRaycastGrounded,isJumpBoostable,isJumpable;
    public bool isBoostable,isDodge,isDodgeable;
    public float boostInput;
    public Vector3 gravityDirection,movementDirection;

    [Header("Stats")]
    public float moveSpeed;
    public float rotationSpeed,jumpForce;

    [Header("Ground")]
    public Vector3 detectGroundVec;
    public float delayAfterFall,multiplyDelayAfterFall,groundDetectRange;

    [Header("Boost")]
    public float currentBoostUsage;
    public float boostUsageMax,boostPower,boostUsageMultiply,boostReplenishMultiply;

    public float currentMoveBoost,selectedMoveBoost;
    public float currentDodgeBoost,dodgeBoostMax,dodgeBoostUsage,dodgeBoostReplenish;

    
    // Start is called before the first frame update
    void Start()
    {
        ikPlayer = GetComponentInChildren<PlayerIk>();
        //plyrCam = Camera.main.GetComponent<PlayerCamera>();
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerSettings>();
        controller = GetComponent<CharacterController>();
        hpSys = GetComponent<HpSystem>();
        cam = Camera.main;

        currentBoostUsage = boostUsageMax;
        currentDodgeBoost = dodgeBoostMax;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position+detectGroundVec,groundDetectRange);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastGrounded();

        if(!hpSys.isDead)
        {
            MovePlayer();
            Boost();
        }
        //Jump();

        isPlayerGrounded = controller.isGrounded;

        delayAfterFall = Mathf.Clamp(delayAfterFall,0,Mathf.Infinity);
        /*if(isRaycastGrounded && delayAfterFall > 0)
        {
            delayAfterFall = delayAfterFall - Time.deltaTime;
        }
        else
        {
            delayAfterFall = delayAfterFall + Time.deltaTime * multiplyDelayAfterFall;
        }*/
        if(delayAfterFall >= 0)
        {
            delayAfterFall = delayAfterFall - Time.deltaTime * multiplyDelayAfterFall;
        }

        Gravity();
        
    }

    void FixedUpdate()
    {
        /*if(currentJerkCooldown > 0)
        {
        Vector3 newVec = new Vector3(Input.GetAxis(control.horizontalInput),0,Input.GetAxis(control.verticalInput)); 
        currentJerkCooldown = currentJerkCooldown - Time.deltaTime*newVec.magnitude;
        }*/
    }

    void MovePlayer()
    {
        Vector3 newVec = new Vector3(Input.GetAxis(control.horizontalInput),0,Input.GetAxis(control.verticalInput));

        //moves player and disables control when in not control mid-air
        if(controller.isGrounded || !controller.isGrounded && newVec.magnitude > 0)
        {
            //rotate parent player to camera
            movementDirection = cam.transform.right * Input.GetAxis(control.horizontalInput) + cam.transform.forward * Input.GetAxis(control.verticalInput);
            movementDirection.y = 0;
            movementDirection.Normalize();
        }

        //controller.Move(movementDirection * moveSpeed * ikPlayer.anim.GetFloat("Movement") * Time.deltaTime);
        controller.Move(movementDirection * moveSpeed * currentMoveBoost * ikPlayer.anim.GetFloat("Movement") * Time.deltaTime);

        //convert a vector into a quaternion
        Vector3 lookDir = Quaternion.Euler(0,cam.transform.rotation.eulerAngles.y,0)*Vector3.forward;

        //whole rotation
        Quaternion lookAng = Quaternion.LookRotation(lookDir,Vector3.up);
        //smoothly rotate transform to "lookAng"
        transform.rotation = Quaternion.Slerp(transform.rotation,lookAng,Time.deltaTime*8);

        //rotate player model to camera

        /*rotDirection = plyrCam.vectorRot;
        //rotDirection.x = 0;
        rotDirection.y = Mathf.Clamp(rotDirection.y,-clampMovementDirection.y,clampMovementDirection.y);
        //rotDirection.z = 0;
        rotDirection.Normalize();

        if (rotDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(0,rotDirection.z,0), Vector3.forward);
            ikPlayer.transform.rotation = Quaternion.Slerp(ikPlayer.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);            
        }*/
    }

    /*void Jump()
    {
        if(Input.GetKeyDown(control.jumpButton) && controller.isGrounded)
        {
            gravityDirection.y += Mathf.Sqrt(jumpForce * -3.0f * Physics.gravity.y);
        }
    }*/

    void Boost()
    {
        currentBoostUsage = Mathf.Clamp(currentBoostUsage,0,boostUsageMax);
        currentDodgeBoost = Mathf.Clamp(currentDodgeBoost,0,dodgeBoostMax);

        currentMoveBoost = Mathf.Lerp(currentMoveBoost,selectedMoveBoost,Time.deltaTime*8);
        currentDodgeBoost = currentDodgeBoost + Time.deltaTime * dodgeBoostReplenish;

        if(Input.GetKey(control.jumpButton) || Input.GetKey(control.boostButton))
        {
            if(currentBoostUsage > 0)
            {
                currentBoostUsage = currentBoostUsage - Time.deltaTime * boostUsageMultiply;

                if(Input.GetKey(control.jumpButton) && isJumpBoostable)
                {
                    gravityDirection.y += boostPower * 10 * Time.deltaTime;
                }

                //if(Input.GetKey(control.boostButton) && isBoostable)
                //{
                    //currentMoveBoost = moveBoost;
                    //controller.Move(movementDirection * boostPower * ikPlayer.anim.GetFloat("Movement") * Time.deltaTime);
                //}
            }
        }
        else
        {
            currentBoostUsage = currentBoostUsage + Time.deltaTime * boostReplenishMultiply;
            selectedMoveBoost = 1;
        }
    }

    void RaycastGrounded()
    {
        Collider[] hitcolliders = Physics.OverlapSphere(transform.position+detectGroundVec,groundDetectRange,groundMask);
        if(!isRaycastGrounded)
        {
            delayAfterFall = -gravityDirection.y;
        }
        
        if(hitcolliders.Length > 0)
        {
            //hasLandedApplyGravity = true;
            isRaycastGrounded = true;
            if(gravityDirection.y < 0)
            {
                gravityDirection.y = -4;
            }
        }
        else
        {
            //hasLandedApplyGravity = false;
            isRaycastGrounded = false;
        }
    }

    void Gravity()
    {
        gravityDirection.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(gravityDirection * Time.deltaTime);
        
        /*if (controller.isGrounded)
        {
            if(gravityDirection.y < 0)
            {
                gravityDirection.y = 0f;
            }
        }*/
    }
}
