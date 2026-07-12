using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    BossIK ikBoss;
    NavMeshAgent ai;
    BoxCollider boxCol;
    PlayerRecticle recticle;
    HpSystem hpSys;
    DebugMode debug;
    [HideInInspector]
    public Transform target,realTarget;

    [Header("RandomDest")]
    public Vector3 minRandDest;
    public Vector3 maxRandDest;
    public float randHeight,heightMin,heightMax,heightAdjust,boxColliderCenterYAdjust;

    [Header("AI")]
    public Vector3 targetOffset;
    public bool isMelee;
    public float aiStoppingRange,rangeToMelee;

    [Header("Retreat")]
    public int hitsToRetreat;
    public bool isRetreat;
    public float retreatTime,currentTimeHits,timeHits;

    void Awake()
    {
        target = GameObject.Find("PlayerAiDest").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<NavMeshAgent>();
        boxCol = GetComponent<BoxCollider>();
        hpSys = GetComponent<HpSystem>();
        recticle = GameObject.FindObjectOfType<PlayerRecticle>();
        ikBoss = GetComponentInChildren<BossIK>();

        debug = GameObject.FindObjectOfType<DebugMode>();
        debug.hpSysBoss = hpSys;
        
        realTarget = GameObject.FindGameObjectWithTag("Player").transform;
        ai.stoppingDistance = aiStoppingRange;
        heightMin = ai.height;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,aiStoppingRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,rangeToMelee);
    }

    // Update is called once per frame
    void Update()
    {
        if(hpSys.isDead)
        {
            ai.height = Mathf.Lerp(ai.height,heightAdjust,Time.deltaTime*2);
            boxCol.center = new Vector3(boxCol.center.x,Mathf.Lerp(boxCol.center.y,realTarget.position.y+randHeight+boxColliderCenterYAdjust,Time.deltaTime*1f),boxCol.center.z);
            ikBoss.transform.position = Vector3.Lerp(ikBoss.transform.position,new Vector3(ikBoss.transform.position.x,heightMin+heightAdjust,ikBoss.transform.position.z),Time.deltaTime);
            ai.enabled = false;
        }
        else
        {
            NavMeshTarget();
            RotLookAt();
            Melee();
            CheckIfHeavilyDamaged();

            if(isRetreat)
            {
                ai.height = Mathf.Lerp(ai.height,realTarget.position.y+randHeight+heightAdjust,Time.deltaTime*2);
                boxCol.center = new Vector3(boxCol.center.x,Mathf.Lerp(boxCol.center.y,realTarget.position.y+randHeight+boxColliderCenterYAdjust,Time.deltaTime*1f),boxCol.center.z);
                ikBoss.transform.position = Vector3.Lerp(ikBoss.transform.position,new Vector3(ikBoss.transform.position.x,realTarget.position.y+randHeight+heightAdjust,ikBoss.transform.position.z),Time.deltaTime);
            }
        }
    }

    void RotLookAt()
    {
        if(ikBoss.aiMovementSpeed > 0)
        {
            Quaternion quat = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,new Quaternion(quat.x,quat.y,0,quat.w),Time.deltaTime*5);
        }
    }

    void BackToOffense()
    {
        isRetreat = false;
    }

    void CheckIfHeavilyDamaged()
    {
        //RaycastHit hit;
        //if(Physics.Raycast(recticle.raycastLookAt.position,recticle.raycastLookAt.forward,out hit,recticle.raycastRange,recticle.enemyMask))
        if(hpSys.numHit > hitsToRetreat)
        {
            RandomizeDestination();
        }

        //check if hit time more than 0
        if(currentTimeHits > 0)
        {
            currentTimeHits = currentTimeHits - Time.deltaTime;
        }
        else
        {
            //check if hit time is less than 0
            hpSys.numHit = 0;
            if(hpSys.isHit)
            {
                currentTimeHits = timeHits;
            }
        }
    }

    public void RandomizeDestination()
    {
        if(!isRetreat)
        {
            ai.SetDestination(new Vector3(Random.Range(minRandDest.x,maxRandDest.x),0,Random.Range(minRandDest.z,maxRandDest.z)));
            randHeight = Random.Range(heightMin,heightMax);
            Invoke("BackToOffense",retreatTime);
            isRetreat = true;
        }
    }

    void Melee()
    {
        if(Vector3.Distance(transform.position,new Vector3(target.position.x,transform.position.y,target.position.z)) < rangeToMelee)
        {
            isMelee = true;
        }
        else
        {
            isMelee = false;
        }
    }

    void NavMeshTarget()
    {
        if(!isRetreat)
        {
            ai.height = Mathf.Lerp(ai.height,realTarget.position.y+heightMin,Time.deltaTime*2);
            boxCol.center = new Vector3(boxCol.center.x,Mathf.Lerp(boxCol.center.y,realTarget.position.y+heightMin+boxColliderCenterYAdjust,Time.deltaTime*1f),boxCol.center.z);
            ikBoss.transform.position = Vector3.Lerp(ikBoss.transform.position,new Vector3(ikBoss.transform.position.x,realTarget.position.y+heightMin+heightAdjust,ikBoss.transform.position.z),Time.deltaTime);
            ai.SetDestination(target.position);
        }
    }
}
