using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HpBarUi : MonoBehaviour
{
    PlayerPause plyrPause;
    public Transform hpUi;
    public Vector3 ogHpBar,hpBarVec;
    public HpSystem hpSys,hpSysPlyr;
    public Image hpBar;
    public Text hpTxt;
    public float hpLerp;
    
    // Start is called before the first frame update
    void Start()
    {
        plyrPause = GameObject.FindObjectOfType<PlayerPause>();
        hpSysPlyr = GameObject.FindGameObjectWithTag("Player").GetComponent<HpSystem>();
        ogHpBar = hpUi.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(hpSys.isHit)
        {
            hpBar.color = Color.red;
        }

        if(plyrPause.isPause || hpSys.isDead || hpSysPlyr.isDead)
        {
            hpUi.localPosition = Vector3.Lerp(hpUi.localPosition,ogHpBar + hpBarVec,Time.unscaledDeltaTime*3);
        }
        else
        {
            hpUi.localPosition = Vector3.Lerp(hpUi.localPosition,ogHpBar,Time.unscaledDeltaTime*3);
        }

        hpBar.color = Color.Lerp(hpBar.color,Color.green,Time.deltaTime*2);
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount,hpSys.hp/hpSys.maxHp/2,Time.deltaTime);

        hpLerp = Mathf.Lerp(hpLerp,hpSys.hp*10/hpSys.maxHp*10,Time.deltaTime*4);
        hpTxt.text = hpLerp.ToString("0") + " %";
    }
    
}
