using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GenericEnemy : MonoBehaviour
{
    NavMeshAgent ai;
    Rigidbody physics;
    Image hitMarker;
    HpSystem hpSys;
    MasterLevel masterLvl;
    AudioSource audioPlayer;
    public AudioClip fireSnd;
    public float stoppingDistanceForAi;

    [Header("Animation")]
    public float aiCurrentSpeed;
    public Animator anim;

    [Header("Transforms")]
    public Transform barrel;
    public Transform head,sightOrigin;
    Vector3 ogBarrelVec;

    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset;

    [Header("Stats")]
    public float hp;
    public float minCooldownFire,maxCooldownFire;
    float currentCooldownFire;

    [Header("Firing FX")]
    public LayerMask shootMask;
    public float rangeToShoot;
    public ParticleSystem bullets,muzzleFlash;

    [Header("Stun")]
    public LayerMask groundMask;
    public float currentTimeStun;

    [Header("Death")]
    public ParticleSystem explosion;
    public AudioClip[] explosionSnds;
    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<NavMeshAgent>();
        hpSys = GetComponent<HpSystem>();
        physics = GetComponent<Rigidbody>();

        ai.stoppingDistance = stoppingDistanceForAi;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        audioPlayer = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioSource>();
        masterLvl = GameObject.FindGameObjectWithTag("MasterLvl").GetComponent<MasterLevel>();
        hitMarker = target.gameObject.GetComponent<PlayerRecticle>().hitMarker;

        if(barrel)
        {
            ogBarrelVec = barrel.position;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,stoppingDistanceForAi);

        Gizmos.color = Color.yellow;
        Vector3 direction = transform.TransformDirection(-sightOrigin.up) * rangeToShoot;
        Gizmos.DrawRay(sightOrigin.position,direction);

        Gizmos.color = Color.white;
        Vector3 direction2 = transform.TransformDirection(-transform.up) * 0.3f;
        Gizmos.DrawRay(transform.position,direction2);
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        Animate();
        HeadFunc();
        Death();

        if(currentTimeStun > 0)
        {
            currentTimeStun = currentTimeStun - Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        NavMeshFunctions();
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(sightOrigin.position,-sightOrigin.up,out hit,rangeToShoot,shootMask) && currentCooldownFire <= 0)
        {
            currentCooldownFire = Random.Range(minCooldownFire,maxCooldownFire);
            bullets.Emit(1);
            muzzleFlash.Emit(5);
            audioPlayer.PlayOneShot(fireSnd,0.1f);

            if(barrel)
            {
                //barrel.position = ogBarrelVec + new Vector3
            }
        }
        else
        {
            currentCooldownFire = currentCooldownFire - Time.deltaTime;
        }
    }

    void Death()
    {
        if(hpSys.hp <= 0)
        {
            masterLvl.currentKillCount++;
            masterLvl.currentNumEnemies--;
            audioPlayer.PlayOneShot(explosionSnds[Random.Range(0,explosionSnds.Length)],0.5f);
            explosion.transform.SetParent(null);
            explosion.Play();
            Destroy(gameObject);
        }
    }

    void HeadFunc()
    {
        head.LookAt(target.position+targetOffset);
    }

    void NavMeshFunctions()
    {
        if(currentTimeStun <= 0)
        {
            if(Physics.Raycast(transform.position,-transform.up,0.3f,groundMask))
            {
                ai.enabled = true;
                physics.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                ai.SetDestination(target.position);
            }
            else
            {
                ai.enabled = false;
            }

            aiCurrentSpeed = ai.velocity.magnitude/ai.speed;
        }
        else
        {
            ai.enabled = false;
            physics.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Animate()
    {
        anim.SetFloat("CurrentSpeed",aiCurrentSpeed);
    }
}
