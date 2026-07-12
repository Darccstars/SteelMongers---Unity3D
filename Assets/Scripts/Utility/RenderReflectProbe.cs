using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderReflectProbe : MonoBehaviour
{
    public ReflectionProbe reflectProbe;
    public bool isRenderStart,isRealtime;
    public float timeRerender;
    // Start is called before the first frame update
    void Start()
    {
        if(isRenderStart)
        {
            Render();
        }
        if(isRealtime)
        {
            InvokeRepeating("Render",0,timeRerender);
        }
        //reflectProbe = GetComponent<ReflectionProbe>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Render()
    {
        reflectProbe.RenderProbe();
    }
}
