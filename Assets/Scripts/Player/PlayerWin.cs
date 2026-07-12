using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWin : MonoBehaviour
{
    Animator anim,plyrAnim;
    PlayerIk plyrIk;
    PlayerGun plyrGun;
    public string returnMainMenuString;
    public bool isPlayVic,isPlyrModelVic;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        plyrIk = GameObject.FindObjectOfType<PlayerIk>();
        plyrAnim = plyrIk.GetComponent<Animator>();
        plyrGun = GameObject.FindObjectOfType<PlayerGun>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlyrModelVic)
        {
            plyrAnim.SetFloat("Hand.R",0);
            plyrIk.weight = 0;
        }

        plyrGun.gameObject.SetActive(!isPlyrModelVic);
        anim.SetBool("isPlayVic",isPlayVic);
        plyrAnim.SetBool("isPlyrModelVic",isPlyrModelVic);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(returnMainMenuString);
    }
}
