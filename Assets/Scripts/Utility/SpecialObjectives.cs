using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObjectives : MonoBehaviour
{
    MasterLevel masterLvl;
    PlayerPause plyrPause;
    GameObject plyr;
    Camera mainCam;
    AudioListener mainCamLis;
    HpSystem hpSys;

    public int levelIndex;

    [Header("Special Objective One")]
    public bool isAllSwitchesTrue;
    public MovePoint entranceDoor;
    [System.Serializable]
    public struct damagedWall
    {
        public Transform wall;
        public ParticleSystem smoke;
        public ParticleSystem debris;
    }
    public damagedWall[] damagedWalls;

    public Switch[] switches;

    [Header("Special Objective Two")]
    //game
    public bool isBossSpawned;
    public GameObject bossEnemy;
    public Transform bossSpawnLoc;
    [ColorUsage(true, true)]
    public Color domeColorAlert,ambientColorAlert,domeTopColorAlert;
    public Texture alertTex;
    public MeshRenderer[] domeParts;
    public bool showTex;
    public float alphaChange;

    [Header("[Cutscene] Special Objective Two")]
    public bool isPlaySceneSPO_Two;
    public float currentDelayCutscene,delayCutscene;
    SpecialObjectives_SPOTwo_Cutscene spObjectiveTwo_Cutscene;
    //public Animator animSPO_Two,bossAnim;
    //public bool isPlaySceneSPO_Two,isJump,isFall,isCutsceneComplete;
    //public ParticleSystem[] thrusters;

    void Awake()
    {
        spObjectiveTwo_Cutscene = GameObject.FindObjectOfType<SpecialObjectives_SPOTwo_Cutscene>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        entranceDoor.enabled = false;
        mainCam = Camera.main;
        mainCamLis = Camera.main.GetComponent<AudioListener>();
        plyr = GameObject.FindGameObjectWithTag("Player");
        hpSys = plyr.GetComponent<HpSystem>();
        masterLvl = GameObject.FindGameObjectWithTag("MasterLvl").GetComponent<MasterLevel>();
        plyrPause = GameObject.FindObjectOfType<PlayerPause>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch(levelIndex)
        {
            case 0:
                SpecialObjectiveOne();
            break;
            case 1:
                //if(!spObjectiveTwo_Cutscene.isCutsceneComplete)
                //{
                    if(!isPlaySceneSPO_Two)
                    {
                        currentDelayCutscene = delayCutscene;
                        isPlaySceneSPO_Two = true;
                    }

                    if(spObjectiveTwo_Cutscene)
                    {
                        plyr.SetActive(false);
                        mainCam.enabled = false;
                        mainCamLis.enabled = false;
                        if(currentDelayCutscene <= 0)
                        {
                            spObjectiveTwo_Cutscene.gameObject.SetActive(true);
                            if(spObjectiveTwo_Cutscene.isCutsceneComplete)
                            {
                                plyr.SetActive(true);
                                mainCam.enabled = true;
                                mainCamLis.enabled = true;
                                Destroy(spObjectiveTwo_Cutscene.gameObject);
                            }
                        }
                        else
                        {
                            currentDelayCutscene = currentDelayCutscene -  Time.deltaTime;
                        }
                    }
                    else
                    {
                        SpecialObjectiveTwo();
                    }
                //}
                //else
                //{
                //    SpecialObjectiveTwo();
                //}
            break;
        }
    }

    void SpecialObjectiveOne()
    {
        for(int x = 0;x<switches.Length;x++)
        {
            if(!switches[0].hasHit || !switches[1].hasHit)
            {
                isAllSwitchesTrue = false;
            }
            else
            {
                if(!isAllSwitchesTrue)
                {
                    entranceDoor.enabled = true;
                    Invoke("BreakWalls",10);
                }
                isAllSwitchesTrue = true;
            }
        }

        /*if(isAllSwitchesTrue)
        {
            entranceDoor.enabled = true;
            Invoke("BreakWalls",10);
        }*/
    }

    public void BreakWalls()
    {
        //levelIndex++;
        masterLvl.SpecialLevelComplete();
        for(int x = 0;x<damagedWalls.Length;x++)
        {
            damagedWalls[x].wall.parent.gameObject.SetActive(false);
            damagedWalls[x].wall.gameObject.SetActive(false);
            damagedWalls[x].smoke.Play();
            damagedWalls[x].debris.Play();
        }
    }

    /*void SpecialObjectiveTwo_Cutscene()
    {
        animSPO_Two.SetBool("isPlayCutscene",true);
        animSPO_Two.SetBool("isCutsceneComplete",isCutsceneComplete);

        bossAnim.SetBool("isFall",isFall);
        bossAnim.SetBool("isJump",isJump);

        Camera.main.enabled = isCutsceneComplete;
    }*/

    void SpecialObjectiveTwo()
    {
        if(!hpSys.isDead)
        {
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight,ambientColorAlert,Time.deltaTime*3);
            for(int x = 0;x<domeParts.Length;x++)
            {
                //if(x == 0)
                //{
                    domeParts[x].material.SetFloat("_AlbedoAlpha",Mathf.Lerp(domeParts[x].material.GetFloat("_AlbedoAlpha"),alphaChange,Time.deltaTime*6));
                    

                    domeParts[x].material.SetTexture("_Albedo",alertTex);
                    domeParts[x].material.SetColor("_Color",Color.Lerp(domeParts[x].material.GetColor("_Color"),domeColorAlert,Time.deltaTime*3));
                    domeParts[x].material.SetVector("_Offset",new Vector2(-0.5f,0));
                //}
                //else
                //{
                //    domeParts[x].material.SetColor("_BaseColor",Color.Lerp(domeParts[x].material.GetColor("_BaseColor"),new Color(domeColorAlert.r,domeColorAlert.g,domeColorAlert.b),Time.deltaTime*3));
                //    domeParts[x].material.SetColor("_EmissionColor",Color.Lerp(domeParts[x].material.GetColor("_EmissionColor"),domeColorAlert,Time.deltaTime*3));
                //}
            }
        }

        if(!isBossSpawned)
        {
            DomeBlinking();
            domeParts[0].GetComponent<Spin>().vecSpeed = new Vector3(0,0,0.5f);
            Instantiate(bossEnemy,bossSpawnLoc.position,bossSpawnLoc.rotation);
            isBossSpawned = true;
        }
    }

    void DomeBlinking()
    {
        showTex = !showTex;
        if(showTex)
        {
            alphaChange = -2;
        }
        else
        {
            alphaChange = 1;
        }
        Invoke("DomeBlinking",0.5f);
    }
}
