using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    Animator anim;
    public float weight,DistanceToGround;
    public Transform holdRightHand,holdRightElbow,holdLeftHand,holdLeftElbow;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        //ik lookat
        //anim.SetLookAtWeight(1,0,weight);
        //anim.SetLookAtPosition(lookAtObj.position);

        //ik hands
        //if(!anim.GetBool("isMelee"))
        //{
        if(holdRightHand)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, holdRightHand.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, holdRightHand.rotation);
        }

        if(holdRightElbow)
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow,weight);
            anim.SetIKHintPosition(AvatarIKHint.RightElbow,holdRightElbow.position);
        }
        //}

        if(holdLeftHand)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
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
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1);

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);

        RaycastHit hit;
        //Right foot
        //create ray and specify its position and facing direction
        Ray rayR = new Ray (anim.GetIKPosition(AvatarIKGoal.RightFoot)+Vector3.up,Vector3.down);
        //operator checking objects within ray's distance and direction
        if(Physics.Raycast(rayR,out hit,DistanceToGround + 1f))
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
        if(Physics.Raycast(rayL,out hit,DistanceToGround + 1f))
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
}
