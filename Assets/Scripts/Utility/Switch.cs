using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool hasHit;
    public MeshRenderer button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if(hasHit)
        {
            button.material.SetColor("_EmissionColor", Color.red * Mathf.Pow(2, 5));
        }
        else
        {
            button.material.SetColor("_EmissionColor", Color.red);
        }
    }

    // Update is called once per frame
    void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.tag == "PlayerBullet")
        {
            hasHit = true;
        }
    }
}
