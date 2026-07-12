using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstructions : MonoBehaviour
{
    Animator anim;
    public bool isPress,isComplete;
    public float timeScale;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        anim.SetBool("isPress",isPress);
        anim.SetBool("isComplete",isComplete);
        if(Input.anyKey)
        {
            isPress = true;
        }
        if(isComplete)
        {
            Destroy(gameObject);
        }
    }
}
