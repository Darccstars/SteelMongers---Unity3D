using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Animator anim;
    MusicManager musicManaging;
    Vector3 ogMenuButtonVec;
    AudioSource sfxPlayer;
    public GameObject musicManager;
    public int selectIndex;
    public bool pressedControl;
    public Transform selectMenuButton;
    public string newgameString;

    [Header("SFX")]
    public AudioClip moveSnd;
    public AudioClip pressClickSnd;

    [System.Serializable]
    public struct mainMenuButton
    {
        public string menuName;
        public Transform menuButton;
        public Image menuButtonImage;
        public Vector3 moveButtonVec;
    }
    public mainMenuButton[] mainMenuButtons;

    [Header("Press Start Game")]
    public bool isPressStartGame;
    public Transform pressStartGameLoc;
    [ColorUsage(true, true)]
    public Color beforeStartColor,afterStartColor;

    [Header("Idle Start Game")]
    public Transform idleGameCamLoc;
    //public Vector3 idleVecPos;
    //public Vector3 idleVecRot;

    [Header("Start Game Pressed")]
    public float blinkTime;
    public Transform startGameCamLoc,playerModel,canvasMenu;
    public Vector3 camOffset,canvasOgVec,canvasOffset;
    public Spin spinCam;
    public bool isStartGame,isStartGameEnd,isBlinkAlertLight,blinkOnOff;
    public MeshRenderer blinkLights;
    [ColorUsage(true, true)]
    public Color glowRed,noGlow,darkAmbientColor,ambientColor;

    [Header("Credits")]
    public bool isOpenCredits;
    public int creditsSelectIndex;
    public Transform creditsTrans,creditsContents;

    [System.Serializable]
    public struct creditElement
    {
        public string menuName;
        public Vector3 moveButtonVec;
    }
    public creditElement[] creditElements;

    public Vector3 ogCreditsContentsVec,selectMenuButtonCreditsOpenVec;

    /*[System.Serializable]
    public struct mainMenuButton
    {
        public string menuName;
        public Transform[] uis;
        public Vector3[] scaleUps,scaleDowns;
        public Vector3 loc,rot;
    }
    public mainMenuButton[] mainMenuButtons;

    public Transform[] oldmainMenuButtons;
    public Vector3[] oldlocs;
    public Vector3[] oldrots;*/
    //public Transform selectCenterPoint;
    // Start is called before the first frame update
    void Start()
    {
        if(!GameObject.FindObjectOfType<MusicManager>())
        {
            GameObject g = Instantiate(musicManager,transform.position,transform.rotation);
            musicManaging = g.GetComponent<MusicManager>();
            print("Create new musicManager");
        }
        else
        {
            musicManaging = GameObject.FindObjectOfType<MusicManager>();
            print("musicManager exist not creating new musicmanager");
        }

        musicManaging.PlayNewMusic(0);

        anim = GetComponent<Animator>();
        sfxPlayer = GetComponent<AudioSource>();

        ogMenuButtonVec = selectMenuButton.transform.localPosition;
        canvasOgVec = canvasMenu.transform.localPosition;
        ogCreditsContentsVec = creditsContents.transform.localPosition;
        canvasMenu.transform.localPosition = canvasOgVec+canvasOffset;
        Camera.main.transform.position = pressStartGameLoc.position;
        Camera.main.transform.rotation = pressStartGameLoc.rotation;
        creditsTrans.localScale = new Vector3(0,0,0);

        Camera.main.transform.SetParent(null);
        RenderSettings.ambientLight = beforeStartColor;

        BlinkingLights();
    }

    void LateUpdate()
    {
        Credits();
        if(isBlinkAlertLight)
        {
            if(blinkOnOff)
            {
                blinkLights.material.SetColor("_EmissionColor",Color.Lerp(blinkLights.material.GetColor("_EmissionColor"),glowRed,Time.deltaTime*15));
                blinkLights.material.SetColor("_BaseColor",Color.Lerp(blinkLights.material.GetColor("_BaseColor"),glowRed,Time.deltaTime*15));
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight,ambientColor,Time.deltaTime*15);
            }
            else
            {
                blinkLights.material.SetColor("_EmissionColor",Color.Lerp(blinkLights.material.GetColor("_EmissionColor"),noGlow,Time.deltaTime*15));
                blinkLights.material.SetColor("_BaseColor",Color.Lerp(blinkLights.material.GetColor("_BaseColor"),noGlow,Time.deltaTime*15));
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight,darkAmbientColor,Time.deltaTime*15);
            }
        }

        if(isStartGameEnd)
        {
            SceneManager.LoadScene(newgameString);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SelectControl();
        EnterButton();

        for(int x = 0;x<mainMenuButtons.Length;x++)
        {
            if(x != selectIndex)
            {
                mainMenuButtons[x].menuButtonImage.color = Color.Lerp(mainMenuButtons[x].menuButtonImage.color,new Color(1,1,1,0.2f),Time.deltaTime*4);
                //mainMenuButtons[x].menuButton.transform.localScale = Vector3.Lerp(mainMenuButtons[x].menuButton.transform.localScale,mainMenuButtons[x].scaleDown,Time.deltaTime*15);
            }
            else
            {
                mainMenuButtons[x].menuButtonImage.color = Color.Lerp(mainMenuButtons[x].menuButtonImage.color,Color.white,Time.deltaTime*4);
                //mainMenuButtons[x].menuButton.transform.localScale = Vector3.Lerp(mainMenuButtons[x].menuButton.transform.localScale,mainMenuButtons[x].scaleUp,Time.deltaTime*15);
            }
        }

        anim.SetBool("isStartGame",isStartGame);
        anim.SetBool("isPressStartGame",isPressStartGame);

        if(isPressStartGame && !isStartGame)
        {
            canvasMenu.transform.localPosition = Vector3.Lerp(canvasMenu.transform.localPosition,canvasOgVec,Time.deltaTime*4);
        }
        else
        {
            canvasMenu.transform.localPosition = Vector3.Lerp(canvasMenu.transform.localPosition,canvasOgVec+canvasOffset,Time.deltaTime*4);
        }

        //after intro is over cam is positioned to idle showing menu
        if(isPressStartGame && !isStartGame)
        {
            Camera.main.transform.SetParent(spinCam.transform);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,idleGameCamLoc.position,Time.deltaTime*5);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation,idleGameCamLoc.rotation,Time.deltaTime*5);
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight,afterStartColor,Time.deltaTime*15);
        }

        if(isStartGame)
        {
            if(!isBlinkAlertLight)
            {
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight,darkAmbientColor,Time.deltaTime*15);
            }
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,startGameCamLoc.position,Time.deltaTime*4);
            Quaternion quat = Quaternion.LookRotation(playerModel.position - Camera.main.transform.position+camOffset);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation,new Quaternion(quat.x,quat.y,0,quat.w),Time.deltaTime*5);

            canvasMenu.transform.localPosition = Vector3.Lerp(canvasMenu.transform.localPosition,canvasOgVec+canvasOffset,Time.deltaTime*4);
            //Camera.main.transform.LookAt(playerModel.position+camOffset);
        }
        /*for(int x = 0;x<mainMenuButtons.Length;x++)
        {
            for(int y = 0;y<mainMenuButtons[x].uis.Length;y++)
            {
                if(x != selectIndex)
                {
                    mainMenuButtons[x].uis[y].transform.localScale = Vector3.Lerp(mainMenuButtons[x].uis[y].transform.localScale,mainMenuButtons[x].scaleDowns[y],Time.deltaTime*9);
                }
                else
                {
                    mainMenuButtons[x].uis[y].transform.localScale = Vector3.Lerp(mainMenuButtons[x].uis[y].transform.localScale,mainMenuButtons[selectIndex].scaleUps[y],Time.deltaTime*9);
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,mainMenuButtons[x].loc,Time.deltaTime*lotSpeed);
                }
                //mainMenuButtons[selectIndex].uis[y].localScale = Vector3.Lerp(mainMenuButtons[selectIndex].uis[y].localScale,mainMenuButtons[selectIndex].scaleUps[y],Time.deltaTime*9);
                
                Quaternion quat = Quaternion.Euler(mainMenuButtons[selectIndex].rot);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation,quat,Time.deltaTime*rotSpeed);
            }
        }*/
        /*for(int x = 0;x<mainMenuButtons.Length;x++)
        {
            if(x != selectIndex)
            {
                mainMenuButtons[x].localScale = Vector3.Lerp(mainMenuButtons[x].localScale,new Vector3(0,1,1),Time.deltaTime*9);
            }
        }
        mainMenuButtons[selectIndex].localScale = Vector3.Lerp(mainMenuButtons[selectIndex].localScale,new Vector3(1,1,1),Time.deltaTime*9);
        //mainMenuTexts[selectIndex].color = Color.Lerp(mainMenuTexts[selectIndex].color,Color.w,Time.deltaTime*3);

        selectCenterPoint.position = Vector3.Lerp(selectCenterPoint.position,locs[selectIndex],Time.deltaTime*lotSpeed);


        Quaternion quat = Quaternion.Euler(rots[selectIndex]);
        selectCenterPoint.rotation = Quaternion.Lerp(selectCenterPoint.rotation,quat,Time.deltaTime*rotSpeed);*/
    }

    void Credits()
    {
        if(isOpenCredits)
        {
            creditsContents.localPosition = Vector3.Lerp(creditsContents.localPosition,ogCreditsContentsVec+creditElements[creditsSelectIndex].moveButtonVec,Time.deltaTime*6);
            selectMenuButton.localPosition = Vector3.Lerp(selectMenuButton.localPosition,ogMenuButtonVec+selectMenuButtonCreditsOpenVec,Time.deltaTime*6);
            //creditsTrans.localScale = new Vector3(Mathf.Lerp(creditsTrans.localScale.x,1,Time.deltaTime*3),Mathf.Lerp(creditsTrans.localScale.y,1,Time.deltaTime*3),Mathf.Lerp(creditsTrans.localScale.z,1,Time.deltaTime*3));
            creditsTrans.localScale = new Vector3(1,Mathf.Lerp(creditsTrans.localScale.y,1,Time.deltaTime*8),1);
        }
        else
        {
            selectMenuButton.localPosition = Vector3.Lerp(selectMenuButton.localPosition,ogMenuButtonVec+mainMenuButtons[selectIndex].moveButtonVec,Time.deltaTime*6);
            //creditsTrans.localScale = new Vector3(Mathf.Lerp(creditsTrans.localScale.x,0,Time.deltaTime*3),Mathf.Lerp(creditsTrans.localScale.y,0,Time.deltaTime*3),Mathf.Lerp(creditsTrans.localScale.z,0,Time.deltaTime*3));
            creditsTrans.localScale = new Vector3(1,Mathf.Lerp(creditsTrans.localScale.y,0,Time.deltaTime*16),1);
        }
    }

    void SelectControl()
    {
        if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
        {
            if(!pressedControl)
            {
                sfxPlayer.PlayOneShot(moveSnd);
                if(!isOpenCredits)
                {
                    if(Input.GetAxis("Horizontal") > 0)
                    {
                        if(selectIndex < mainMenuButtons.Length-1)
                        {
                            selectIndex++;
                        }
                        else
                        {
                            selectIndex = 0;
                        }
                    }
                    else
                    {
                        if(selectIndex > 0)
                        {
                            selectIndex--;
                        }
                        else
                        {
                            selectIndex = mainMenuButtons.Length-1;
                        }
                    }
                }
                else
                {
                    if(Input.GetAxis("Horizontal") > 0)
                    {
                        if(creditsSelectIndex < creditElements.Length-1)
                        {
                            creditsSelectIndex++;
                        }
                        else
                        {
                            creditsSelectIndex = 0;
                        }
                    }
                    else
                    {
                        if(creditsSelectIndex > 0)
                        {
                            creditsSelectIndex--;
                        }
                        else
                        {
                            creditsSelectIndex = creditElements.Length-1;
                        }
                    }
                }
            }
            pressedControl = true;
        }
        else
        {
            pressedControl = false;
        }
    }

    public void EnterButton()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            sfxPlayer.PlayOneShot(pressClickSnd);
            switch(selectIndex)
            {
                case 0:
                    isStartGame = true;
                    spinCam.enabled = false;
                    //SceneManager.LoadScene(newgameString);
                break;
                case 1:
                    isOpenCredits = !isOpenCredits;
                break;
                case 2:
                    Application.Quit();
                break;
            }
        }
    }

    public void CloseCredits()
    {
        isOpenCredits = false;
    }

    void BlinkingLights()
    {
        Invoke("BlinkingLights",blinkTime);
        if(isBlinkAlertLight)
        {
            blinkOnOff = !blinkOnOff;
        }
    }

    void PlaySndSFX(AudioClip sfx)
    {
        sfxPlayer.PlayOneShot(sfx);
    }
}
