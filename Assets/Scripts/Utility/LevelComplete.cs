using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    Animator plyrAnim;
    public Transform[] camLocs;
    public bool isLevelComplete;

    // Start is called before the first frame update
    void Start()
    {
        plyrAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLevelComplete)
        {
            plyrAnim.SetBool("isVictory",true);
        }
    }
}
