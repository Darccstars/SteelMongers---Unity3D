using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    BossEnemy boss;
    HpSystem hpSys;
    Transform target;
    AudioSource audioPlayer;
    public AudioClip fireSnd;
    
    [Header("Raycast")]
    public Transform raycastOrigin;
    public float rangeToShoot;
    public LayerMask enemyMask;

    [Header("Particles")]
    public int numBullets;
    public float currentCooldown,minCooldown,maxCooldown;
    public ParticleSystem projectile,muzzleFire;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossEnemy>();
        hpSys = GetComponentInParent<HpSystem>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        audioPlayer = GetComponent<AudioSource>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(raycastOrigin.position,raycastOrigin.forward*rangeToShoot);
    }

    // Update is called once per frame
    void Update()
    {
        if(!hpSys.isDead)
        {
            AimGun();
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(raycastOrigin.position,raycastOrigin.forward,out hit,rangeToShoot,enemyMask) && currentCooldown <= 0)
        {
            projectile.Emit(numBullets);
            muzzleFire.Emit(15);
            audioPlayer.PlayOneShot(fireSnd,1);
            currentCooldown = Random.Range(minCooldown,maxCooldown);
        }
        else
        {
            currentCooldown = currentCooldown - Time.deltaTime;
        }
    }

    void AimGun()
    {
        //Quaternion quat = Quaternion.LookRotation(boss.target.position + boss.targetOffset - transform.position);
        //transform.rotation = new Quaternion(quat.x,quat.y,quat.z,quat.w);
        transform.LookAt(boss.targetOffset+target.position);
        //gunTransform.LookAt(plyrIk.lookAtObj.position);
    }
}
