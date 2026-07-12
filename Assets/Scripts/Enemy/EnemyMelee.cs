using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    HpSystem playerHpSys;
    Collider selfCol;
    public float dmg;
    // Start is called before the first frame update
    void Start()
    {
        playerHpSys = GameObject.FindGameObjectWithTag("Player").GetComponent<HpSystem>();
        selfCol = GetComponent<Collider>();
        selfCol.enabled = false;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            playerHpSys.hp -= dmg;
        }
    }
}
