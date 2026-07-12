using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIk : MonoBehaviour
{
    [HideInInspector]
    public Animator anim;
    public Transform weapon;
    public PlayerCamera plyrCam;
    public BoxCollider meleeBox;
    PlayerPause plyrPause;
    AudioSource audioPlayer;
    ControllerSettings control;
    PlayerMain plyrMain;
    CharacterController controller;

    [Header("IK Settings")]
    public LayerMask groundIkMask;
    public float weight,DistanceToGround;

    [Header("IK Bones")]
    public Transform lookAtObj;
    public Transform holdRightHand,holdRightElbow,holdLeftHand,holdLeftElbow;

    [Header("Change Rot")]
    public Vector3 currentRot;
    public Vector3 speedRotVector,lerpRotVector;
    //public Vector3 changeHorVertRot;
    //public Vector3 changeHorRot;

    [System.Serializable]
    public struct thruster
    {
        public ParticleSystem fireVernier;
        public ParticleSystem blastVernier;

        public bool isRotateable;
        public Vector3 currentRotVec,limitRotPos,limitRotNeg;
    }
    [Header("Thrusters")]
    public thruster[] thrusters;
    public float boostLean;

    [Header("Smoke")]
    public ParticleSystem landingSmoke;
    //0 = left, 1 = right
    public ParticleSystem[] smokeTrails,smokeFootsteps;

    [System.Serializable]
    public struct shakeSetting
    {
        public string actionName;
        public float shakeDuration,shakeAmount;
    }
    [Header("Shake Settings")]
    public shakeSetting[] shakeSettings;

    [System.Serializable]
    public struct sndLibrary
    {
        public string audioName;
        public AudioClip[] audioSFX;
        public float volume;
    }
    [Header("AudioSFX")]
    public sndLibrary[] sndLibraries;

    [System.Serializable]
    public struct footstep
    {
        public string audioTypeName;
        public LayerMask collisionMask;
        public AudioClip[] audioSFX;
        public float volume;
    }
    public footstep[] footsteps;
    public Transform groundDetector;

    [System.Serializable]
    public struct particleDeath
    {
        public ParticleSystem[] particlesOnDeath;
    }
    [Header("Hp")]
    public particleDeath[] particlesDeath;
    
    public ParticleSystem[] particlesDamaged;
    HpSystem hpSys;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        plyrMain = GetComponentInParent<PlayerMain>();
        hpSys = GetComponentInParent<HpSystem>();

        anim = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();

        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerSettings>();
        plyrPause = GameObject.FindObjectOfType<PlayerPause>();
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimationVars();
        if(!hpSys.isDead)
        {
            RotateModel();
            VernierRot();
            SmokeTrail();
            //ParticlesOnDamage();

            if(Input.GetKeyDown(control.boostButton) )
            {
                Boosting(1);
            }
        }
        else
        {
            weight = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(groundDetector.position,groundDetector.position+new Vector3(0,-3,0));
    }

    void SetAnimationVars()
    {
        Vector3 newVec = new Vector3(Input.GetAxis(control.horizontalInput),0,Input.GetAxis(control.verticalInput));

        if(!plyrPause.isPause)
        {
            anim.SetFloat("Input",newVec.magnitude);
        }

        anim.SetBool("isDead",hpSys.isDead);
        anim.SetFloat("Vertical",Input.GetAxis(control.verticalInput));
        anim.SetFloat("Horizontal",Input.GetAxis(control.horizontalInput));
        anim.SetFloat("VerticalForce",plyrMain.gravityDirection.y);
        anim.SetBool("isGrounded",controller.isGrounded);
        anim.SetBool("isRaycastGrounded",plyrMain.isRaycastGrounded);
        anim.SetFloat("DelayAfterFall",plyrMain.delayAfterFall);
        anim.SetBool("isJumpBoostable",plyrMain.isJumpBoostable);
        anim.SetBool("JumpBoostingInput",Input.GetKey(control.jumpButton));
        anim.SetBool("isJumpable",plyrMain.isJumpable);
        anim.SetBool("BoostInput",Input.GetKey(control.boostButton));
        anim.SetBool("isBoostable",plyrMain.isBoostable);
        anim.SetBool("MeleeTrigger",Input.GetKey(control.meleeButton));
        anim.SetFloat("isShooting",Input.GetAxis(control.fireInput));

        weapon.gameObject.SetActive(!anim.GetBool("isDead"));

        if(Input.GetKeyDown(control.jumpButton) && plyrMain.isRaycastGrounded && !hpSys.isDead)
        {
            anim.SetTrigger("isJump");
        }

        /*if(!anim.GetBool("isMelee"))
        {
            anim.SetFloat("Hand.R",Mathf.Lerp(anim.GetFloat("Hand.R"),1,Time.deltaTime*3));
        }
        else
        {
            anim.SetFloat("Hand.R",0);
        }*/
    }

    void RotateModel()
    {
        if(anim.GetFloat("Movement") > 0 && !plyrPause.isPause)
        {
            if(Input.GetAxis(control.verticalInput) != 0 || Input.GetAxis(control.horizontalInput) != 0)
            {
                if(Input.GetAxis(control.verticalInput) > -0.001f)
                {
                    Quaternion newQuat = Quaternion.LookRotation(plyrMain.movementDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation,newQuat,Time.fixedDeltaTime*8);
                }
                else
                {
                    Quaternion newQuat = Quaternion.LookRotation(new Vector3(-plyrMain.movementDirection.x,0,-plyrMain.movementDirection.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation,newQuat,Time.fixedDeltaTime*8);
                }
            }

            /*if(Input.GetKey(control.boostButton) && plyrMain.isBoostable && Input.GetAxis(control.verticalInput) < 0)
            {
                Quaternion toRotation = Quaternion.Euler(new Vector3(-plyrMain.movementDirection.magnitude*boostLean,transform.rotation.y,0));
                transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, 1 * Time.deltaTime);
            }*/
        }

        /*currentRot.x += Input.GetAxis(control.verticalInput)*200*Time.deltaTime*25;
        currentRot.x = Mathf.Clamp(currentRot.x,-45,45);
        currentRot.y += Input.GetAxis(control.horizontalInput)*speedRotVector.y*Time.deltaTime*lerpRotVector.y;
        currentRot.y = Mathf.Clamp(currentRot.y,-90,90);

        Quaternion toRotation = Quaternion.Euler(0,currentRot.y,0);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, plyrMain.rotationSpeed * Time.deltaTime);*/

        /*if(Input.GetAxis(control.horizontalInput) != 0 && Input.GetAxis(control.verticalInput) != 0)
        {
            Quaternion toRotation = Quaternion.Euler(new Vector3(transform.localRotation.x,Input.GetAxis(control.verticalInput) * Input.GetAxis(control.horizontalInput) * changeHorVertRot.y,0));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, plyrMain.rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion toRotation = Quaternion.Euler(new Vector3(transform.localRotation.x,Input.GetAxis(control.horizontalInput) * changeHorRot.y,0));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, plyrMain.rotationSpeed * Time.deltaTime);
        }*/

        /*if(Input.GetKey(control.boostButton) && plyrMain.isBoostable)
        {
            Quaternion toRotation = Quaternion.Euler(new Vector3(Input.GetAxis(control.verticalInput)*boostLean,transform.localRotation.y,0));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, plyrMain.rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion toRotation = Quaternion.Euler(new Vector3(0,transform.localRotation.y,0));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, plyrMain.rotationSpeed * Time.deltaTime);
        }*/
    }

    void OnAnimatorIK(int layerIndex)
    {
        //ik lookat
        anim.SetLookAtWeight(weight,0,weight);
        anim.SetLookAtPosition(lookAtObj.position);

        //ik hands
        //if(!anim.GetBool("isMelee"))
        //{
        if(holdRightHand)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, anim.GetFloat("Hand.R"));
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, anim.GetFloat("Hand.R"));
            anim.SetIKPosition(AvatarIKGoal.RightHand, holdRightHand.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, holdRightHand.rotation);
        }

        if(holdRightElbow)
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow,anim.GetFloat("Hand.R"));
            anim.SetIKHintPosition(AvatarIKHint.RightElbow,holdRightElbow.position);
        }
        //}

        if(holdLeftHand)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, holdLeftHand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, holdLeftHand.rotation);
        }

        if(holdLeftElbow)
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,1);
            anim.SetIKHintPosition(AvatarIKHint.LeftElbow,holdLeftElbow.position);
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
        if(Physics.Raycast(rayR,out hit,DistanceToGround + 1f,groundIkMask))
        {
            //create new vector based on the surface it touches
            Vector3 footPosition = hit.point;
            //create an offset between the foot and surface
            footPosition.y += DistanceToGround;
            //place the foot's vector with "footPosition" and apply rotation adapting to the surface's floor
            anim.SetIKPosition(AvatarIKGoal.RightFoot,footPosition);
            anim.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.LookRotation(transform.forward,hit.normal));
            //FootstepsSFX();
        }

        //Left foot
        //create ray and specify its position and facing direction
        Ray rayL = new Ray (anim.GetIKPosition(AvatarIKGoal.LeftFoot)+Vector3.up,Vector3.down);
        //operator checking objects within ray's distance and direction
        if(Physics.Raycast(rayL,out hit,DistanceToGround + 1f,groundIkMask))
        {
            //create new vector based on the surface it touches
            Vector3 footPosition = hit.point;
            //create an offset between the foot and surface
            footPosition.y += DistanceToGround;
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

    /*void ParticlesOnDamage()
    {
        //play first particle if less than 3/4 hp
        if(hpSys.hp < hpSys.maxHp*3/4)
        {
            PlayParticlesOnDamage(0);
        }
        else
        {
            StopParticlesOnDamage(0);
        }

        if(hpSys.hp < hpSys.maxHp*2/4)
        {
            PlayParticlesOnDamage(1);
        }
        else
        {
            StopParticlesOnDamage(1);
        }

        if(hpSys.hp < hpSys.maxHp*1/4)
        {
            PlayParticlesOnDamage(2);
        }
        else
        {
            StopParticlesOnDamage(2);
        }
    }

    void PlayParticlesOnDamage(int index)
    {
        if(!particlesDamaged[index].isPlaying)
        {
            particlesDamaged[index].Play();
        }
    }

    void StopParticlesOnDamage(int index)
    {
        particlesDamaged[index].Stop();
    }*/

    void ShakyCam(int shakyCond)
    {
        plyrCam.shakeDuration = shakeSettings[shakyCond].shakeDuration;
        plyrCam.shakeAmount = shakeSettings[shakyCond].shakeAmount;
    }

    void Melee(int intMelee)
    {
        switch(intMelee)
        {
            case 0:
                anim.SetBool("isMelee",false);
            break;
            case 1:
                anim.SetBool("isMelee",true);
                anim.SetFloat("Hand.R",Mathf.Lerp(anim.GetFloat("Hand.R"),1,Time.deltaTime));
            break;
        }
    }

    void Boostable(int boostIndex)
    {
        switch(boostIndex)
        {
            case 0:
                plyrMain.isBoostable = false;
            break;
            case 1:
                plyrMain.isBoostable = true;
            break;
        }
    }

    void Jumpable(int jumpIndex)
    {
        switch(jumpIndex)
        {
            case 0:
                plyrMain.isJumpable = false;
            break;
            case 1:
                plyrMain.isJumpable = true;
            break;
        }
    }

    void VernierRot()
    {
        for(int x = 0;x<thrusters.Length;x++)
        {
            if(thrusters[x].isRotateable)
            {
                //if(Input.GetAxis(control.verticalInput) != 0 || Input.GetAxis(control.horizontalInput) != 0)
                thrusters[x].currentRotVec.x += Input.GetAxis(control.verticalInput) * 200 * Time.deltaTime;
                thrusters[x].currentRotVec.y += Input.GetAxis(control.horizontalInput) * 200 * Time.deltaTime;

                thrusters[x].currentRotVec.x = Mathf.Clamp(thrusters[x].currentRotVec.x,thrusters[x].limitRotNeg.x,thrusters[x].limitRotPos.x);
                thrusters[x].currentRotVec.y = Mathf.Clamp(thrusters[x].currentRotVec.y,thrusters[x].limitRotNeg.y,thrusters[x].limitRotPos.y);
                //rotVec.y = Mathf.Clamp(rotVec.y,-thrusters[x].limitRotNeg.y,thrusters[x].limitRotPos.y);

                Quaternion toRotation = Quaternion.Euler(thrusters[x].currentRotVec.x,thrusters[x].currentRotVec.y,0);
                thrusters[x].fireVernier.transform.parent.localRotation = Quaternion.Slerp(thrusters[x].fireVernier.transform.parent.localRotation, toRotation, Time.deltaTime * 10);  
            }
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

    void LandingSmoke(int particleAmount)
    {
        landingSmoke.Emit(particleAmount);
    }

    void SmokeFootStep(int footIndex)
    {
        smokeFootsteps[footIndex].Emit(20);
    }

    void SmokeTrail()
    {
        if(Input.GetKey(control.boostButton))
        {
            if(controller.isGrounded)
            {
                if(Input.GetAxis(control.verticalInput) != 0 || Input.GetAxis(control.horizontalInput) != 0)
                {
                    for(int x = 0;x<smokeTrails.Length;x++)
                    {
                        smokeTrails[x].Play();
                    }
                }
            }
            else
            {
                for(int x = 0;x<smokeTrails.Length;x++)
                {
                    smokeTrails[x].Stop();
                }
            }
        }
        else
        {
            for(int x = 0;x<smokeTrails.Length;x++)
            {
                smokeTrails[x].Stop();
            }
        }
    }

    void JumpBoostable(int index)
    {
        switch(index)
        {
            case 0:
                plyrMain.isJumpBoostable = false;
            break;
            case 1:
                plyrMain.isJumpBoostable = true;
            break;
        }
    }

    void PlaySoundIfThrustersPlaying(int soundIndex)
    {
        for(int x = 0;x<thrusters.Length;x++)
        {
            if(thrusters[x].fireVernier.isPlaying)
            {
                PlaySounds(soundIndex);
            }
        }
    }

    void VernierRotAnimEvent(float vecRot)
    {
        for(int x = 0;x<thrusters.Length;x++)
        {
            if(thrusters[x].isRotateable)
            {
                thrusters[x].currentRotVec.x = vecRot;
            }
        }
    }

    public void VernierBoost(int playIndex)
    {
        for(int x = 0;x<thrusters.Length;x++)
        {
            switch (playIndex)
            {
                case 0:
                    thrusters[x].fireVernier.Play();
                break;
                case 1:
                    thrusters[x].fireVernier.Stop();
                break;
            }
        }
    }

    void VernierBlast()
    {
        for(int x = 0;x<thrusters.Length;x++)
        {
            thrusters[x].blastVernier.Play();
        }
    }

    void PlayFootSnds()
    {
        if(plyrMain.isRaycastGrounded)
        {
            for(int x = 0;x<footsteps.Length;x++)
            {
                if(Physics.Raycast(groundDetector.position,-Vector3.up,3,footsteps[x].collisionMask))
                {
                    audioPlayer.PlayOneShot(footsteps[x].audioSFX[Random.Range(0,footsteps[x].audioSFX.Length)],footsteps[x].volume);
                }
            }
        }
    }

    void PlaySounds(int chooseSndType)
    {
        audioPlayer.PlayOneShot(sndLibraries[chooseSndType].audioSFX[Random.Range(0,sndLibraries[chooseSndType].audioSFX.Length)],sndLibraries[chooseSndType].volume);
    }

    void Boosting(int boostType)
    {
        switch(boostType)
        {
            case 0:
                plyrMain.selectedMoveBoost = plyrMain.boostPower;
            break;
            case 1:
            if(plyrMain.currentDodgeBoost >= plyrMain.dodgeBoostUsage && plyrMain.currentBoostUsage > 0)
            {
                plyrMain.selectedMoveBoost = plyrMain.boostPower * 3;
                plyrMain.currentDodgeBoost -= plyrMain.dodgeBoostUsage;
                VernierBlast();
                PlaySounds(2);
            }
            break;
        }
    }

    void JumpForce()
    {
        plyrMain.gravityDirection.y += Mathf.Sqrt(plyrMain.jumpForce * -3.0f * Physics.gravity.y);
    }
}
