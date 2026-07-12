using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MonoBehaviour
{
    AudioSource audioPlayer;
    ControllerSettings control;
    PlayerCamera plyrCam;
    PlayerIk plyrIk;
    Vector3 vectorRot,ogGunBoltVec;
    HpSystem hpSys;
    
    public Transform gunTransform,gunBolt;
    public ParticleSystem projectile,muzzleFire,shells;

    Light muzzleFireLight;
    float muzzleFireLightOgIntensity;
    public AudioClip fireSnd,shellHitSnd;

    [Header("Stats")]
    public float cooldown;
    float currentCooldown;

    [Header("Readjust Gun Position")]
    public Vector3 currentReadjustGunPos;
    public Vector3 readjustGunPos;

    [Header("Recoil")]
    public PlayerRecticle recticle;
    public float recoilRecoverLerp,recticleRecoil;
    public Vector3 recoilPos,recoilRot;
    Vector3 originalVec;

    //public float lookXrotLimit;
    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControllerSettings>();
        hpSys = GameObject.FindGameObjectWithTag("Player").GetComponent<HpSystem>();
        plyrCam = Camera.main.GetComponent<PlayerCamera>();
        plyrIk = GetComponentInParent<PlayerIk>();
        audioPlayer = GetComponent<AudioSource>();

        muzzleFireLight = muzzleFire.GetComponent<Light>();
        muzzleFireLightOgIntensity = muzzleFireLight.intensity;
        muzzleFireLight.intensity = 0;

        originalVec = gunTransform.localPosition;
        ogGunBoltVec = gunBolt.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hpSys.isDead)
        {
            AimGun();
            FireGun();
        }
        GunOnOff();
    }
    
    void LateUpdate()
    {
        ReadjustGunPosition();
    }

    void ReadjustGunPosition()
    {
        if(Input.GetAxis(control.horizontalInput) != 0 && Input.GetAxis(control.verticalInput) == 0)
        {
            currentReadjustGunPos = new Vector3(readjustGunPos.x,readjustGunPos.y,readjustGunPos.z * Input.GetAxis(control.horizontalInput));
        }
        else
        {
            currentReadjustGunPos = new Vector3(0,0,0);
        }
    }

    void GunOnOff()
    {
        if(plyrIk.anim.GetBool("isMelee"))
        {
            foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        }
        else
        {
            foreach (Transform child in transform)
            child.gameObject.SetActive(true);
        }
    }

    void AimGun()
    {
        /*Vector3 gunToIkLook = plyrIk.lookAtObj.position - transform.position;
        Quaternion quatAngle = Quaternion.LookRotation(gunToIkLook);
        transform.rotation = Quaternion.Slerp(transform.rotation,quatAngle,Time.deltaTime*10);*/
        //transform.LookAt(plyrIk.lookAtObj.position);
        Quaternion quat = Quaternion.LookRotation(plyrIk.lookAtObj.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation,quat,Time.deltaTime*45);
        //gunTransform.LookAt(plyrIk.lookAtObj.position);
    }

    void FireGun()
    {
        if(Input.GetAxis(control.fireInput) > 0 && currentCooldown <= 0)
        {
            audioPlayer.PlayOneShot(fireSnd,0.75f);
            projectile.Emit(1);
            shells.Emit(1);
            muzzleFire.Emit(15);
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, gunTransform.localPosition + recoilPos + currentReadjustGunPos, Time.deltaTime*5);
            gunBolt.localPosition = ogGunBoltVec + new Vector3(0,0,-0.001f);
            recticle.ReceiveRecoil(recticleRecoil);
            muzzleFireLight.intensity = muzzleFireLightOgIntensity;

            Quaternion shootRot = Quaternion.Euler(recoilRot);
            gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation,shootRot,Time.deltaTime*10);

            currentCooldown = cooldown;
        }
        else
        {
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, originalVec + currentReadjustGunPos, Time.deltaTime*recoilRecoverLerp);
            gunBolt.localPosition = Vector3.Lerp(gunBolt.localPosition, ogGunBoltVec, Time.deltaTime*5);
            muzzleFireLight.intensity = Mathf.Lerp(muzzleFireLight.intensity,0,Time.deltaTime*5);

            Quaternion restRot = Quaternion.Euler(0,0,0);
            gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation,restRot,Time.deltaTime*recoilRecoverLerp);

            currentCooldown = currentCooldown - Time.deltaTime;
        }
    }
}
