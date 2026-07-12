using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFPS : MonoBehaviour
{
    public Text fpsDisplay;
    public int changeFPS;
    private float count;
    
    private IEnumerator Start()
    {
        //DontDestroyOnLoad(transform.parent.gameObject);
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        Application.targetFrameRate = changeFPS;
        fpsDisplay.text = "FPS : " + Mathf.Round(count);
    }
}
