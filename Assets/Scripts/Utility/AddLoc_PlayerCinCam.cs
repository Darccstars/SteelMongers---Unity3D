using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddLoc_PlayerCinCam : MonoBehaviour
{
    PlayerCinematicCamera plyrCinCam;

    [System.Serializable]
    public struct addCinematicSetting
    {
        public Transform loc;
        public bool isReparentCam,isReparentLocs,isLerp,isRot;
    }
    public List<addCinematicSetting> addCinematicSettings; 
    // Start is called before the first frame update
    void Start()
    {
        plyrCinCam = GameObject.FindObjectOfType<PlayerCinematicCamera>();
        for(int x = 0;x<addCinematicSettings.Count;x++)
        {
            var cd = new PlayerCinematicCamera.cinematicSetting();
            cd.loc = addCinematicSettings[x].loc;
            cd.isReparentCam = addCinematicSettings[x].isReparentCam;
            cd.isReparentLocs = addCinematicSettings[x].isReparentLocs;
            cd.isLerp = addCinematicSettings[x].isLerp;
            cd.isRot = addCinematicSettings[x].isRot;
            plyrCinCam.cinematicSettings.Add(cd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
