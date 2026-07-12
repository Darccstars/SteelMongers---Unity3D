using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOff : MonoBehaviour
{
    ControllerSettings control;
    public ParticleSystem[] particles;
    public float currentCooldown,cooldown;
    public bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerSettings>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(control.boostButton))
        {
            if(currentCooldown <= 0)
            {
                isActive = !isActive;
                for(int x = 0;x<particles.Length;x++)
                {
                    particles[x].gameObject.SetActive(isActive);
                    if(!particles[x].isPlaying)
                    {
                        particles[x].Play();
                    }
                }
                currentCooldown = cooldown;
            }
            else
            {
                currentCooldown = currentCooldown - Time.deltaTime;
            }
        }
        else
        {
            if(isActive)
            {
                for(int x = 0;x<particles.Length;x++)
                {
                    particles[x].gameObject.SetActive(false);
                    isActive = false;
                }
            }
            currentCooldown = 0;
        }
        
    }
}
