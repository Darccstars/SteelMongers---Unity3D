using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    HpSystem hpSys;
    PlayerPause plyrPause;
    ControllerSettings control;
    PlayerMain plyrMain;
    PlayerInstructions plyrInstructions;
    DebugMode debug;
    float hpLerp,hpLerpHundred;

    public Image hpBar,boostGauge,dodgeBoostGauge;
    public Text hpPercentText;
    Color ogBoostGaugeColor,ogDodgeBoostGauge;

    [System.Serializable]
    public struct gameUI
    {
        public Transform ui;
        public Vector3 ogUiVec,uiVecOffset;
    }
    public gameUI[] gameUis;
    
    // Start is called before the first frame update
    void Start()
    {
        debug = GameObject.FindObjectOfType<DebugMode>();
        plyrPause = GameObject.FindObjectOfType<PlayerPause>();
        plyrInstructions = GameObject.FindObjectOfType<PlayerInstructions>();
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerSettings>();
        hpSys = GetComponent<HpSystem>();

        plyrMain = GetComponent<PlayerMain>();
        ogBoostGaugeColor = boostGauge.color;
        ogDodgeBoostGauge = dodgeBoostGauge.color;

        for(int x = 0;x<gameUis.Length;x++)
        {
            gameUis[x].ogUiVec = gameUis[x].ui.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RearrangeUI();
        boostGauge.fillAmount = Mathf.Lerp(boostGauge.fillAmount,plyrMain.currentBoostUsage/plyrMain.boostUsageMax,Time.deltaTime*4);
        dodgeBoostGauge.fillAmount = Mathf.Lerp(dodgeBoostGauge.fillAmount,plyrMain.currentDodgeBoost/plyrMain.dodgeBoostMax,Time.deltaTime*4);
        
        hpLerp = Mathf.Lerp(hpLerp,hpSys.hp*10/hpSys.maxHp*10,Time.deltaTime*4);
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount,hpSys.hp/hpSys.maxHp,Time.deltaTime*4);
        hpPercentText.text = hpLerp.ToString("0") + " %";

        if(Input.GetKey(control.jumpButton) || Input.GetKey(control.boostButton))
        {
            boostGauge.color = Color.white;
        }
        else
        {
            boostGauge.color = Color.Lerp(boostGauge.color,ogBoostGaugeColor,Time.deltaTime*4);
        }

        if(Input.GetKeyDown(control.boostButton))
        {
            if(plyrMain.currentDodgeBoost >= plyrMain.dodgeBoostUsage && plyrMain.currentBoostUsage > 0)
            {
                dodgeBoostGauge.color = Color.white;
            }
        }
        else
        {
            if(plyrMain.currentDodgeBoost >= plyrMain.dodgeBoostUsage && plyrMain.currentBoostUsage > 0)
            {
                dodgeBoostGauge.color = Color.Lerp(dodgeBoostGauge.color,ogDodgeBoostGauge,Time.deltaTime*4);
            }
            else
            {
                dodgeBoostGauge.color = Color.Lerp(dodgeBoostGauge.color,Color.red,Time.deltaTime*4);
            }
        }
    }

    void RearrangeUI()
    {
        if(!plyrInstructions)
        {
            if(plyrPause.isPause || hpSys.isDead || debug.isHideUi)
            {
                UIMove(0);
            }
            else
            {
                UIMove(1);
            }
        }
        else
        {
            if(plyrInstructions.isPress)
            {
                UIMove(1);
            }
            else
            {
                UIMove(0);
            }
        }
    }

    void UIMove(int index)
    {
        for(int x = 0;x<gameUis.Length;x++)
        {
            switch(index)
            {
                case 0:
                    gameUis[x].ui.localPosition = Vector3.Lerp(gameUis[x].ui.localPosition,gameUis[x].ogUiVec+gameUis[x].uiVecOffset,Time.unscaledDeltaTime*2);
                break;
                case 1:
                    gameUis[x].ui.localPosition = Vector3.Lerp(gameUis[x].ui.localPosition,gameUis[x].ogUiVec,Time.unscaledDeltaTime*10);
                break;
            }
        }
    }
}
