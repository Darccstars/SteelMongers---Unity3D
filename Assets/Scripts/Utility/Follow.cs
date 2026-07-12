using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float lerpSpeed;
    public bool isFollowRot,isDeparent;
    // Start is called before the first frame update
    void Start()
    {
        if(isDeparent)
        {
            transform.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            transform.position = Vector3.Lerp(transform.position,target.position+offset,Time.deltaTime*lerpSpeed);
            //transform.rotation = target.rotation;
        }
    }
}
