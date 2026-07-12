using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObjectives_SPOTwo_Cutscene : MonoBehaviour
{
    //SpecialObjectives spObjectives;
    [HideInInspector]
    public Animator animSPO_Two;
    public Camera cinematicCam;
    public Transform holeCover;

    [System.Serializable]
    public struct camShot
    {
        public Transform loc,locLookAt;
        public Vector3 offsetLocLookAt;
        [HideInInspector]
        public Quaternion quat;
        public bool isLerp;
        public float lerpPos,lerpRot;
    }
    [Header("Camera Settings")]
    public int camIndex;
    public float shakeDuration,shakeIntensity;
    public camShot[] camShots;

    [System.Serializable]
    public struct camShake
    {
        public float shakeDuration,shakeIntensity;
    }
    public camShake[] camShakes;

    [Header("Animation")]
    public bool isJump;
    public bool isFall,isCutsceneComplete,isHoleGone;

    [Header("Boss Model")]
    public Animator bossAnim;
    //public float DistanceToGround,weight;
    //public Transform holdRightHand,holdRightElbow,holdLeftHand,holdLeftElbow;
    
    // Start is called before the first frame update
    void Start()
    {
        animSPO_Two = GetComponent<Animator>();
        //cinematicCam = GetComponentInChildren<Camera>();
        //cinematicCam.transform.SetParent(null);
        //spObjectives = GameObject.FindObjectOfType<SpecialObjectives>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        animSPO_Two.SetBool("isCutsceneComplete",isCutsceneComplete);

        bossAnim.SetBool("isFall",isFall);
        bossAnim.SetBool("isJump",isJump);

        shakeDuration = Mathf.Clamp(shakeDuration,0,100);
        if(shakeDuration > 0)
        {
            shakeDuration -= 1 * Time.deltaTime;
        }

        holeCover.gameObject.SetActive(!isHoleGone);

        if(camShots[camIndex].isLerp)
        {
            cinematicCam.transform.position = Vector3.Lerp(cinematicCam.transform.position,camShots[camIndex].loc.position+Random.insideUnitSphere * shakeIntensity * shakeDuration,Time.deltaTime*camShots[camIndex].lerpPos);
            if(camShots[camIndex].locLookAt)
            {
                camShots[camIndex].quat = Quaternion.LookRotation(camShots[camIndex].locLookAt.position - cinematicCam.transform.position+camShots[camIndex].offsetLocLookAt);
                cinematicCam.transform.rotation = Quaternion.Slerp(cinematicCam.transform.rotation,camShots[camIndex].quat,Time.deltaTime*camShots[camIndex].lerpRot);
            }
            else
            {
                cinematicCam.transform.rotation = Quaternion.Lerp(cinematicCam.transform.rotation,camShots[camIndex].loc.rotation,Time.deltaTime*camShots[camIndex].lerpRot);
            }
        }
        else
        {
            cinematicCam.transform.position = camShots[camIndex].loc.position;
            if(camShots[camIndex].locLookAt)
            {
                cinematicCam.transform.LookAt(camShots[camIndex].loc.position);
            }
            else
            {
                cinematicCam.transform.rotation = camShots[camIndex].loc.rotation;
            }
        }
        
        if(Camera.main)
        {
            Camera.main.enabled = isCutsceneComplete;
        }
    }

    public void ShakeCamera(int shakeIndex)
    {
        shakeDuration = camShakes[shakeIndex].shakeDuration;
        shakeIntensity = camShakes[shakeIndex].shakeIntensity;
    }
}
