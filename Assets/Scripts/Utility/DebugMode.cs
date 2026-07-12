using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMode : MonoBehaviour
{
    PlayerCinematicCamera plyrCinCam;
    MasterLevel masterLvl;
    SpecialObjectives spObjectives;
    SetFPS fpsSetting;
    [HideInInspector]
    public HpSystem hpSys,hpSysBoss;
    public bool isDebug,isUnlockCursor,showFps,isHideUi,confirmDebug;
    public Transform[] relocateGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        fpsSetting = GameObject.FindObjectOfType<SetFPS>();
        plyrCinCam = GameObject.FindObjectOfType<PlayerCinematicCamera>();
        if(!isUnlockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if(!showFps)
        {
            fpsSetting.enabled = false;
            fpsSetting.fpsDisplay.enabled = false;
        }

        hpSys = GameObject.FindGameObjectWithTag("Player").GetComponent<HpSystem>();
        if(GameObject.FindGameObjectWithTag("MasterLvl"))
        {
            masterLvl = GameObject.FindGameObjectWithTag("MasterLvl").GetComponent<MasterLevel>();
            spObjectives = GameObject.FindGameObjectWithTag("SpecialObjectives").GetComponent<SpecialObjectives>();
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Backspace) && Input.GetKey(KeyCode.Return))
        {
            if(!confirmDebug)
            {
                confirmDebug = true;
                isDebug = !isDebug;
            }
        }
        else
        {
            confirmDebug = false;
        }

        if(isDebug)
        {
            if(masterLvl)
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    if(masterLvl.currentLevel < masterLvl.levelEnemies.Length-1)
                    {
                        if(spObjectives.levelIndex == 0 && masterLvl.currentLevel == 1)
                        {
                            spObjectives.BreakWalls();
                        }
                        else
                        {
                            masterLvl.SpecialLevelComplete();
                        }
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                plyrCinCam.isCinematic = !plyrCinCam.isCinematic;
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                hpSys.hp = 999999;
                hpSys.maxHp = 999999;
            }
            if(Input.GetKeyDown(KeyCode.P))
            {
                hpSys.hp = 0;
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if(Input.GetKeyDown(KeyCode.I))
            {
                isHideUi = !isHideUi;
            }

            if(hpSysBoss)
            {
                if(Input.GetKeyDown(KeyCode.O))
                {
                    hpSysBoss.hp = 0;
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for(int x = 0;x<relocateGameObjects.Length;x++)
        {
            if(relocateGameObjects[x].position.y <= -10)
            {
                if(relocateGameObjects[x].GetComponent<CharacterController>())
                {
                    relocateGameObjects[x].GetComponent<CharacterController>().enabled = false;
                    relocateGameObjects[x].position = new Vector3(0,0,0);
                    relocateGameObjects[x].GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    relocateGameObjects[x].position = new Vector3(0,0,0);
                }
            }
        }

        /*if(Input.GetKeyDown(KeyCode.E))
        {
            if(masterLvl.currentLevel < masterLvl.levelEnemies.Length-1)
            {
                if(!masterLvl.levelEnemies[masterLvl.currentLevel].specialCondition)
                {
                    masterLvl.SpecialLevelComplete();
                }
                else
                {
                    masterLvl.SpecialLevelComplete();
                    spObjectives.levelIndex++;
                }
            }
        }*/
    }
}
