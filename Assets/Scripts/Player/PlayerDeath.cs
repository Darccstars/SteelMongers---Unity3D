using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    HpSystem hpSys;
    PlayerMain plyrMain;
    PlayerIk plyrIk;
    PlayerCamera plyrCam;
    Transform mainCamParent;
    //PlayerPause plyrPause;
    public Text pauseText,continueText;
    public bool isGameOver,isGameOverEnd;
    public float delayGameOverScreen;
    //public Vector3 ogPlyrCamVec;
    public Spin spinScript;
    public Transform deathCamLoc;
    //public Vector3 camPosOffset;
    [ColorUsage(true, true)]
    public Color blackAmbient;
    // Start is called before the first frame update
    void Start()
    {
        hpSys = GetComponent<HpSystem>();
        plyrMain = GetComponent<PlayerMain>();
        plyrIk = GetComponentInChildren<PlayerIk>();
        //plyrPause = GameObject.FindObjectOfType<PlayerPause>();
        
        mainCamParent = Camera.main.transform.parent;
        plyrCam = Camera.main.GetComponent<PlayerCamera>();
        //ogMainCamVec = mainCam.transform.position;
        //ogPlyrCamVec = plyrCam.transform.localPosition;

        //normalAmbient = RenderSettings.ambientLight;
        spinScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(hpSys.isDead)
        {
            plyrIk.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            plyrIk.VernierBoost(1);
            Quaternion quat = Quaternion.Euler(new Vector3(5,mainCamParent.eulerAngles.y,0));
            //mainCamParent.rotation =  Quaternion.Lerp(plyrCam.transform.rotation,quat,Time.deltaTime*3);
            plyrIk.anim.SetFloat("Hand.R",0);
            mainCamParent.position = Vector3.Lerp(mainCamParent.position,deathCamLoc.position,Time.deltaTime*3);
            //plyrCam.transform.localPosition = Vector3.Lerp(plyrCam.transform.localPosition,ogPlyrCamVec+camPosOffset,Time.deltaTime*3);
            //Quaternion quat = Quaternion.Euler(new Vector3(0,0,0));
            //plyrCam.transform.rotation = Quaternion.Lerp(plyrCam.transform.rotation,quat,Time.deltaTime*3);

            //Time.timeScale = Mathf.Lerp(Time.timeScale,0.01f,Time.unscaledDeltaTime*7);
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight,blackAmbient,Time.unscaledDeltaTime*3);
            spinScript.enabled = true;
            plyrMain.enabled = false;
            plyrCam.enabled = false;
            if(!isGameOver)
            {
                pauseText.text = "Game Over";
                continueText.text = "Restart";
                Invoke("GameOverScreen",delayGameOverScreen);
                isGameOver = true;
            }
        }
    }

    void GameOverScreen()
    {
        isGameOverEnd = true;
    }
}
