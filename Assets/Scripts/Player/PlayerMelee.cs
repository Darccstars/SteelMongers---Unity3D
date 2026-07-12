using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    Transform player;
    Collider selfCol;
    public float dmg,kickForceForward,kickForceUp,timeStun;

    void Start()
    {
        selfCol = GetComponent<Collider>();
        selfCol.enabled = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<HpSystem>() && col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<HpSystem>().hp -= dmg;

            if(col.gameObject.GetComponent<GenericEnemy>())
            {
                col.gameObject.GetComponent<GenericEnemy>().currentTimeStun = timeStun;
            }
        }

        if(col.gameObject.GetComponent<Rigidbody>())
        {
            Rigidbody physics = col.gameObject.GetComponent<Rigidbody>();
            physics.AddForce(player.forward*kickForceForward);
            physics.AddForce(player.up*kickForceUp);
        }
    }
}
