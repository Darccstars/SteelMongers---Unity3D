using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossIK : MonoBehaviour
{
    NavMeshAgent ai;
    BossEnemy boss;
    Animator anim;
    HpSystem hpSys;
    AudioSource audioPlayer;
    public BoxCollider meleeBox;
    public float aiMovementSpeed;

    [Header("IK")]
    public float weight;
    public Vector3 headOffset;
    Transform target;
    public LayerMask groundIkMask;
    public float distanceToGround,rightHandWeight;
    public Transform holdRightHand;
    public Transform holdRightElbow;

    [Header("Thrusters")]
    public ParticleSystem[] thrusters;

    [System.Serializable]
    public struct particleDeath
    {
        public ParticleSystem[] particlesOnDeath;
    }
    [Header("ParticlesPlay")]
    public particleDeath[] particlesDeath;

     [System.Serializable]
    public struct sndLibrary
    {
        public string audioName;
        public AudioClip[] audioSFX;
    }
    [Header("AudioSFX")]
    public sndLibrary[] sndLibraries;


    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossEnemy>();
        ai = GetComponentInParent<NavMeshAgent>();
        hpSys = GetComponentInParent<HpSystem>();
        anim = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        target = boss.target;
        
    }

    // Update is called once per frame
    void Update()
    {
        ThrustersFunc();
        anim.SetFloat("CurrentSpeed",aiMovementSpeed);
        anim.SetBool("IsMelee",boss.isMelee);
        anim.SetBool("isDead",hpSys.isDead);

        if(hpSys.isDead)
        {
            weight = 0;
        }

        aiMovementSpeed = Mathf.Lerp(aiMovementSpeed,ai.velocity.magnitude/ai.speed,Time.deltaTime*9);
    }

    void ThrustersFunc()
    {
        if(aiMovementSpeed > 0.1f)
        {
            for(int x = 0;x<thrusters.Length;x++)
            {
                if(!thrusters[x].isPlaying)
                thrusters[x].Play();
            }
        }
        else
        {
            for(int x = 0;x<thrusters.Length;x++)
            {
                thrusters[x].Stop();
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        //ik lookat
        anim.SetLookAtWeight(weight,0,weight);
        anim.SetLookAtPosition(target.parent.position+headOffset);

        if(holdRightHand)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, holdRightHand.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, holdRightHand.rotation);
        }

        if(holdRightElbow)
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow,1);
            anim.SetIKHintPosition(AvatarIKHint.RightElbow,holdRightElbow.position);
        }

        //foot IK
        //apply ik weight to both foots
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot,anim.GetFloat("footIK.L"));
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,anim.GetFloat("footIK.L"));

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot,anim.GetFloat("footIK.R"));
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot,anim.GetFloat("footIK.R"));

        RaycastHit hit;
        //Right foot
        //create ray and specify its position and facing direction
        Ray rayR = new Ray (anim.GetIKPosition(AvatarIKGoal.RightFoot)+Vector3.up,Vector3.down);
        //operator checking objects within ray's distance and direction
        if(Physics.Raycast(rayR,out hit,distanceToGround + 1f,groundIkMask))
        {
            //create new vector based on the surface it touches
            Vector3 footPosition = hit.point;
            //create an offset between the foot and surface
            footPosition.y += distanceToGround;
            //place the foot's vector with "footPosition" and apply rotation adapting to the surface's floor
            anim.SetIKPosition(AvatarIKGoal.RightFoot,footPosition);
            anim.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.LookRotation(transform.forward,hit.normal));
            //FootstepsSFX();
        }

        //Left foot
        //create ray and specify its position and facing direction
        Ray rayL = new Ray (anim.GetIKPosition(AvatarIKGoal.LeftFoot)+Vector3.up,Vector3.down);
        //operator checking objects within ray's distance and direction
        if(Physics.Raycast(rayL,out hit,distanceToGround + 1f,groundIkMask))
        {
            //create new vector based on the surface it touches
            Vector3 footPosition = hit.point;
            //create an offset between the foot and surface
            footPosition.y += distanceToGround;
            //place the foot's vector with "footPosition" and apply rotation adapting to the surface's floor
            anim.SetIKPosition(AvatarIKGoal.LeftFoot,footPosition);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.LookRotation(transform.forward,hit.normal));
            //FootstepsSFX();
        }
    }

    void DeathPlayParticles(int index)
    {
        for(int x = 0;x<particlesDeath[index].particlesOnDeath.Length;x++)
        {
            particlesDeath[index].particlesOnDeath[x].Play();
        }
    }

    public void EnableDisableMeleeBox(int enableDisable)
    {
        switch(enableDisable)
        {
            case 0:
                meleeBox.enabled = false;
            break;
            case 1:
                meleeBox.enabled = true;
            break;
        }
    }

    void RandDest()
    {
        boss.RandomizeDestination();
    }

    void PlaySounds(int chooseSndType)
    {
        audioPlayer.PlayOneShot(sndLibraries[chooseSndType].audioSFX[Random.Range(0,sndLibraries[chooseSndType].audioSFX.Length)]);
    }
}
