using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath : MonoBehaviour
{
    Camera mainCam;
    AudioListener mainCamListener;
    BossWeapon bossWeap;
    BossIK bossIk;
    Animator bossDeathAnim,bossAnim;
    HpSystem bossHpSys;
    PlayerWin plyrWin;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        mainCamListener = Camera.main.GetComponent<AudioListener>();
        bossDeathAnim = GetComponent<Animator>();
        bossHpSys = GetComponentInParent<HpSystem>();

        bossAnim = transform.parent.GetComponentInChildren<Animator>();
        bossIk = bossAnim.GetComponent<BossIK>();

        plyrWin = GameObject.FindObjectOfType<PlayerWin>();
        bossWeap = GameObject.FindObjectOfType<BossWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if(bossHpSys.isDead)
        {
            bossDeathAnim.SetBool("isDeath",true);
            bossIk.rightHandWeight = 0;
            mainCam.gameObject.SetActive(false);
            mainCamListener.enabled = false;
            bossWeap.gameObject.SetActive(false);
        }
    }

    void PlayPlayerVictoryAnim()
    {
        plyrWin.isPlayVic = true;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
