using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target,parentCam;
    public PlayerIk playerIK;

    Volume vol;
    MotionBlur blurFX;

    PlayerMain plyrMain;
    ControllerSettings control;
    Camera cam;
    PlayerCinematicCamera cinCam;

    [Header("Camera Pos")]
    public Vector3 vectorCamOffset;
    public float camPosLerp;

    [Header("Set Ik Pos")]
    public LayerMask collisionMask;
    public Transform ikLookTrans;
    public Vector3 ikLookOffset;

    [Header("Shake Settings")]
    public float shakeAmount;
    public float shakeDuration;
    //[Header("Readjust Targeting")]
    //public Vector3 targetingCorrection;

    [Header("Mouse Settings")]
    public float mouseSpeed;
    public float mouseLerp,lookUpRotLimit;
    public Vector3 vectorRot;
    float mouseX,mouseY,currentMouseSpeed,ogMouseSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        cinCam = GameObject.FindObjectOfType<PlayerCinematicCamera>();
        plyrMain = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerSettings>();

        vol = GetComponent<Volume>();
        vol.profile.TryGet(out blurFX);

        parentCam.SetParent(null);
        cam = Camera.main;
    }

    // Update is called once per frame
    //void Update()
    //{
        //CamFollow();
        //ogMouseSpeed = mouseSpeed;
    //}

    void LateUpdate()
    {
        ogMouseSpeed = mouseSpeed;
        CamFollow();
        shakeDuration = Mathf.Clamp(shakeDuration,0,100);
        if(shakeDuration > 0)
        {
            shakeDuration -= 1 * Time.deltaTime;
        }
        if(playerIK.anim.GetBool("BoostInput") && !cinCam.isCinematic)
        {
            blurFX.active = true;
        }
        else
        {
            blurFX.active = false;
        }

        CamCollision();
        SetIKLookAt();
        RotateCam();
    }

    void RotateCam()
    {
        mouseX = Input.GetAxis(control.mouseX);
        mouseY = Input.GetAxis(control.mouseY);

        //Input to vector
        vectorRot.x += mouseX * currentMouseSpeed * Time.deltaTime;
        vectorRot.y += mouseY * currentMouseSpeed * Time.deltaTime;

        if(Input.GetAxis(control.focusAimButton)>0)
        {
            currentMouseSpeed = ogMouseSpeed*1/4;
        }
        else
        {
            currentMouseSpeed = ogMouseSpeed;
        }

        //Limit looking up
        vectorRot.y  = Mathf.Clamp(vectorRot.y,-lookUpRotLimit,lookUpRotLimit);

        //restart y rotation to 0 when below/above 360 degrees
        if(Mathf.Abs(vectorRot.x)>360)
        {
            vectorRot.x = 0;
        }

        //smooth rotation cam
        Quaternion quatAngle = Quaternion.Euler(-vectorRot.y,vectorRot.x,0);
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation,quatAngle,Time.deltaTime*mouseLerp);
    }

    void CamFollow()
    {
        //parentCam.position = Vector3.Slerp(parentCam.position,target.position,Time.deltaTime*camPosLerp);
        //transform.parent.position = Vector3.Slerp(transform.parent.position,target.transform.position+Random.insideUnitSphere * shakeAmount * shakeDuration,Time.deltaTime*5);
        transform.parent.position = Vector3.Slerp(transform.parent.position,target.transform.position+Random.insideUnitSphere * shakeAmount * shakeDuration,Time.deltaTime*camPosLerp);
    }

    void SetIKLookAt()
    {
        //RaycastHit hit;
        //set the far point with raycasy(i.e) set the look position
        /*if(Physics.Raycast(transform.position+ikLookOffsetWorld,transform.forward,out hit,200,collisionMask))
        {
            //if obstructed set look position to the surface of obstruction
            ikLookTrans.position = Vector3.Lerp(ikLookTrans.position,hit.point,Time.deltaTime*10);
        }
        else
        {*/
            //ikLookTrans.localPosition = Vector3.Lerp(ikLookTrans.localPosition,ikLookOffset + new Vector3(Input.GetAxis(control.horizontalInput)*targetingCorrection.x*plyrMain.moveSpeed,Input.GetAxis(control.verticalInput)*targetingCorrection.y*plyrMain.moveSpeed,0),Time.deltaTime*10);
            ikLookTrans.localPosition = Vector3.Lerp(ikLookTrans.localPosition,ikLookOffset,Time.deltaTime*10);
        //}
    }

    void CamCollision()
    {
        //set origin to target
        Vector3 origin = target.transform.position;
        //create a ray which takes "origin" position and negate the "origin" vector with current position
        Ray r = new Ray (origin, -(origin - transform.position ).normalized);
        RaycastHit hit;
        //check collision between "origin" and current position
        if(Physics.Linecast(origin,transform.position,out hit,collisionMask))
        {
            //apply current position based on surface distance and direction
            //transform.position = origin + r.direction * hit.distance;
            transform.position = Vector3.Lerp(transform.position,origin + r.direction * hit.distance,Time.deltaTime*6);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition,vectorCamOffset,Time.deltaTime*3);
            Quaternion quat = Quaternion.Euler(0,0,0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation,quat,Time.deltaTime*3);
        }
    }
}
