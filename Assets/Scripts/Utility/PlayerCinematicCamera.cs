using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCinematicCamera : MonoBehaviour
{
    Camera cinCam,mainCam;
    Transform player,cinCamParent,cinCamLocsParent;

    public int camIndex;
    public bool isCinematic;

    [System.Serializable]
    public struct cinematicSetting
    {
        public Transform loc;
        public bool isReparentCam,isReparentLocs,isLerp,isRot;
    }
    public List<cinematicSetting> cinematicSettings;

    //public Transform[] cinematicLocs;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        cinCam = GetComponent<Camera>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        cinCamParent = cinCam.transform.parent;
        cinCamLocsParent = cinematicSettings[0].loc.parent;

        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        cinCam.enabled = isCinematic;
        mainCam.enabled = !isCinematic;

        if(Input.GetKeyDown(KeyCode.Y))
        {
            if(camIndex < cinematicSettings.Count-1)
            {
                camIndex++;
            }
            else
            {
                camIndex = 0;
            }
        }
    }

    void LateUpdate()
    {
        //for(int x = 0;x<cinematicSettings.Length;x++)
        //{
            //purpose: make the camera follow with/without rotation affected by parent
            if(cinematicSettings[camIndex].isReparentCam)
            {
                transform.SetParent(cinCamParent);
                //transform.position = Vector3.Lerp(transform.position,cinematicSettings[x].loc.position,Time.deltaTime*4);
                //transform.rotation = Quaternion.Lerp(transform.rotation,cinematicSettings[x].loc.rotation,Time.deltaTime*4);
            }
            else
            {
                //unparents the cam so rotation is independent
                transform.SetParent(null);
                if(cinematicSettings[camIndex].isLerp)
                {
                    transform.position = Vector3.Lerp(transform.position,cinematicSettings[camIndex].loc.position,Time.unscaledDeltaTime*15);
                    transform.rotation = Quaternion.Lerp(transform.rotation,cinematicSettings[camIndex].loc.rotation,Time.unscaledDeltaTime*15);
                }
                else
                {
                    transform.position = cinematicSettings[camIndex].loc.position;
                    transform.rotation = cinematicSettings[camIndex].loc.rotation;
                }
                //transform.rotation = new Quaternion();
            }

            //purpose: make the cam locations parent connected to the player follow with/without rotation affected by parent
            if(cinematicSettings[camIndex].isReparentLocs)
            {
                cinCamLocsParent.SetParent(cinCamParent);
            }
            else
            {
                cinCamLocsParent.SetParent(null);
                if(cinematicSettings[camIndex].isLerp)
                {
                    cinCamLocsParent.position = Vector3.Lerp(cinCamLocsParent.position,player.position,Time.unscaledDeltaTime*15);
                }
                else
                {
                    cinCamLocsParent.position = player.position;
                }
                if(cinematicSettings[camIndex].isRot)
                {
                    cinCamLocsParent.rotation = Quaternion.Lerp(cinCamLocsParent.rotation,player.rotation,Time.unscaledDeltaTime*15);
                }
            }
        //}
        //transform.position = Vector3.Lerp(transform.position,cinematicLocs[camIndex].position,Time.deltaTime*4);
        //transform.rotation = Quaternion.Lerp(transform.rotation,cinematicLocs[camIndex].rotation,Time.deltaTime*4);
    }
}
