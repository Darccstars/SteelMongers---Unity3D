using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecticle : MonoBehaviour
{
    public Transform raycastLookAt,target;
    public Image hitMarker;
    public float raycastRange;
    public LayerMask enemyMask;

    [System.Serializable]
    public struct recticle
    {
        public Image recticleImg;
        public Vector3 recticleOgVec;
        public Vector3 maxOffsetRecoil;
    }
    public recticle[] recticles;

    //public Transform[] recticles;
    //public Vector2[] recticleOgVec;
    //public Vector3 maxOffsetRecoil;
    public float recoilRecoveryLerp;
    // Start is called before the first frame update
    void Start()
    {
        /*for(int x = 0;x<recticles.Length;x++)
        {
            recticles[x].recticleOgVec = recticles[x].recticleTrans.position;
        }*/
    }

    void OnDrawGizmosSelected()
    {
        //Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward) * raycastRange;
        Debug.DrawRay(raycastLookAt.position, raycastLookAt.forward*raycastRange, Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        raycastLookAt.LookAt(target);

        hitMarker.color = Color.Lerp(hitMarker.color,new Color(0,0,0,0),Time.deltaTime*4);

        for(int x = 0;x<recticles.Length;x++)
        {
            recticles[x].recticleImg.transform.localPosition = Vector2.Lerp(recticles[x].recticleImg.transform.localPosition,recticles[x].recticleOgVec,Time.deltaTime*recoilRecoveryLerp);

            RaycastHit hit;
            if(Physics.Raycast(raycastLookAt.position,raycastLookAt.forward,out hit,raycastRange,enemyMask))
            {
                recticles[x].recticleImg.color = Color.red;
            }
            else
            {
                recticles[x].recticleImg.color = Color.Lerp(recticles[x].recticleImg.color,Color.white,Time.deltaTime*4);
            }
        }
    }

    public void ReceiveRecoil(float recoilAmt)
    {
        for(int x = 0;x<recticles.Length;x++)
        {
            recticles[x].recticleImg.transform.localPosition = recticles[x].recticleOgVec+recticles[x].maxOffsetRecoil*recoilAmt;
        }
    }
}
