using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject spawn;
    public Transform spawnLoc;

    [Header("Cooldown")]
    public float currentCooldown;
    public float cooldown;

    [Header("Randomize Cooldown")]
    public bool randomizeCooldown;
    public float minCooldown,maxCooldown;

    // Start is called before the first frame update
    void Start()
    {
        if(!spawnLoc)
        {
            spawnLoc = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCooldown <= 0)
        {
            Instantiate(spawn,spawnLoc.position,spawnLoc.rotation);
            if(randomizeCooldown)
            {
                currentCooldown = Random.Range(minCooldown,maxCooldown);
            }
            else
            {
                currentCooldown = cooldown;
            }
        }
        else
        {
            currentCooldown = currentCooldown - Time.deltaTime;
        }
    }
}
