using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpSystem : MonoBehaviour
{
    public bool isDead;
    public float hp;
    [HideInInspector]
    public float maxHp;
    [System.Serializable]
    public struct colTag
    {
        public string vulnerableToMask;
        public float hpDmg;
    }
    public colTag[] colTags;

    [Header("For Boss")]
    public int numHit;
    public bool usesIsHit,isHit;
    
    // Start is called before the first frame update
    void Start()
    {
        maxHp = hp;
    }

    // Update is called once per frame
    void Update()
    {
        hp = Mathf.Clamp(hp,0,maxHp);
        if(hp <= 0)
        {
            isDead = true;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        for(int x = 0;x<colTags.Length;x++)
        {
            if(other.gameObject.tag == colTags[x].vulnerableToMask)
            {
                hp -= colTags[x].hpDmg;

                if(usesIsHit)
                {
                    numHit++;
                    if(!isHit)
                    {
                        isHit = true;
                        Invoke("HitConfirmed",0.1f);
                    }
                }
            }
        }
    }

    void HitConfirmed()
    {
        isHit = false;
    }
}
