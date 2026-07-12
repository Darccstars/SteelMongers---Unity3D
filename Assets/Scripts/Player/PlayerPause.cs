using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    Animator anim;
    ControllerSettings control;
    Vector3 ogMenuButtonVec;
    PlayerCamera plyrCam;
    PlayerMain plyrMain;
    PlayerDeath plyrDeath;
    PlayerIk plyrIk;
    PlayerGun plyrGun;
    MusicManager musicManaging;
    AudioSource sfxPlayer;

    public GameObject musicManagerObj;
    [Header("SFX")]
    public AudioClip moveSnd;
    public AudioClip pressClickSnd;
    //public PlayerInstructions plyrInstructions;
    [Header("Transforms")]
    public Transform selectMenuButton,pauseCamLoc;
    public Vector3 vecSelectMenuButtonUnpaused;
    [Header("Pause Vars")]
    public int selectIndex;
    public bool isPause,isRestart,isExit,isExitConfirmed,pressedControl;
    public string backToMainMenuString;

    [System.Serializable]
    public struct mainMenuButton
    {
        public string menuName;
        public Transform menuButton;
        public Image menuButtonImage;
        public Vector3 moveButtonVec;
    }
    public mainMenuButton[] mainMenuButtons;

    void Awake()
    {
        if(GameObject.FindObjectOfType<MusicManager>())
        {
            musicManaging = GameObject.FindObjectOfType<MusicManager>();
        }
        else
        {
            GameObject g = Instantiate(musicManagerObj,transform.position,transform.rotation);
            musicManaging = g.GetComponent<MusicManager>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindObjectOfType<ControllerSettings>();
        plyrCam = GameObject.FindObjectOfType<PlayerCamera>();
        plyrMain = GameObject.FindObjectOfType<PlayerMain>();
        plyrIk = GameObject.FindObjectOfType<PlayerIk>();
        plyrGun = GameObject.FindObjectOfType<PlayerGun>();
        musicManaging = GameObject.FindObjectOfType<MusicManager>();
        plyrDeath = GameObject.FindObjectOfType<PlayerDeath>();
        sfxPlayer = GetComponent<AudioSource>();
        //plyrInstructions = GameObject.FindObjectOfType<PlayerInstructions>();
        ogMenuButtonVec = selectMenuButton.localPosition;
        selectMenuButton.localPosition = ogMenuButtonVec+vecSelectMenuButtonUnpaused;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isPause",isPause);
        anim.SetBool("isExit",isExit);
        anim.SetBool("isRestart",isRestart);
        anim.SetBool("isGameOver",plyrDeath.isGameOverEnd);
        Controls();

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(isPause || plyrDeath.isGameOverEnd)
            {
                Selection();
            }
        }
    }

    void Controls()
    {
        if(isPause || plyrDeath.isGameOverEnd)
        {
            if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
            {
                if(!pressedControl)
                {
                    sfxPlayer.PlayOneShot(moveSnd);
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
                pressedControl = true;
            }
            else
            {
                pressedControl = false;
            }

            if(isExitConfirmed)
            {
                SceneManager.LoadScene(backToMainMenuString);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    void Reenable()
    {
        plyrMain.enabled = true;
        plyrGun.enabled = true;
    }

    void LateUpdate()
    {
        if(!musicManaging.isReplaceMusic)
        {
            //check if exit not pause not gameover, not exit is pause,
            if(isPause && !isExit && !isRestart || plyrDeath.isGameOverEnd && !isExit && !isRestart)
            //if(isExit && !isPause || !isExit && isPause || plyrDeath.isGameOverEnd && !isRestart)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale,0.01f,Time.unscaledDeltaTime*3);
                plyrIk.weight = Mathf.Lerp(plyrIk.weight,0,Time.unscaledDeltaTime*3);
                if(!plyrDeath.isGameOverEnd)
                {
                    plyrCam.transform.position = Vector3.Lerp(plyrCam.transform.position,pauseCamLoc.position,Time.unscaledDeltaTime*3);
                    plyrCam.transform.rotation = Quaternion.Lerp(plyrCam.transform.rotation,pauseCamLoc.rotation,Time.unscaledDeltaTime*3);
                }
                selectMenuButton.localPosition = Vector3.Lerp(selectMenuButton.localPosition,ogMenuButtonVec+mainMenuButtons[selectIndex].moveButtonVec,Time.unscaledDeltaTime*6);
                musicManaging.audioPlay.volume = Mathf.Lerp(musicManaging.audioPlay.volume,musicManaging.pauseVol,Time.unscaledDeltaTime*3);
            }
            else
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale,1,Time.unscaledDeltaTime*3);
                plyrIk.weight = Mathf.Lerp(plyrIk.weight,1,Time.unscaledDeltaTime*3);
                selectMenuButton.localPosition = Vector3.Lerp(selectMenuButton.localPosition,ogMenuButtonVec+vecSelectMenuButtonUnpaused,Time.unscaledDeltaTime*6);
                musicManaging.audioPlay.volume = Mathf.Lerp(musicManaging.audioPlay.volume,musicManaging.musicVol,Time.unscaledDeltaTime*3);
            }
        }

        for(int x = 0;x<mainMenuButtons.Length;x++)
        {
            if(x != selectIndex)
            {
                mainMenuButtons[x].menuButtonImage.color = Color.Lerp(mainMenuButtons[x].menuButtonImage.color,new Color(1,1,1,0.2f),Time.unscaledDeltaTime*4);
                //mainMenuButtons[x].menuButton.transform.localScale = Vector3.Lerp(mainMenuButtons[x].menuButton.transform.localScale,mainMenuButtons[x].scaleDown,Time.deltaTime*15);
            }
            else
            {
                mainMenuButtons[x].menuButtonImage.color = Color.Lerp(mainMenuButtons[x].menuButtonImage.color,Color.white,Time.unscaledDeltaTime*4);
                //mainMenuButtons[x].menuButton.transform.localScale = Vector3.Lerp(mainMenuButtons[x].menuButton.transform.localScale,mainMenuButtons[x].scaleUp,Time.deltaTime*15);
            }
        }
    }

    void Selection()
    {
        sfxPlayer.PlayOneShot(pressClickSnd);
        switch(selectIndex)
        {
            case 0:
                if(!plyrDeath.isGameOver)
                {
                    PauseUnpause();
                }
                else
                {
                    isRestart = true;
                }
            break;
            case 1:
                isExit = true;
            break;
        }
    }

    void PauseUnpause()
    {
        if(!isExit && !plyrDeath.isGameOver)
        {
            isPause = !isPause;
            if(isPause)
            {
                plyrCam.enabled = false;
                plyrMain.enabled = false;
                plyrGun.enabled = false;
            }
            else
            {
                plyrCam.enabled = true;
                Invoke("Reenable",0.5f);
            }
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
