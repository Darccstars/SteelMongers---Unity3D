using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToGround : MonoBehaviour
{
    public Transform parent;
    public Vector3 offset,parentOriginOffset;
    public float range;
    public LayerMask groundMask;
    public bool isAdjustFixedToWorldVec;
    public Vector3 fixWorldVec;
    // Start is called before the first frame update
    void Start()
    {
        if(transform.parent == null)
        {
            parent = transform;
        }
        else
        {
            parent = transform.parent;
        }
    }

    void OnDrawGizmosSelected()
    {
        if(parent)
        {
            Gizmos.DrawRay(parent.position+parentOriginOffset,-parent.up*range);
        }
        else
        {
            Gizmos.DrawRay(transform.position+parentOriginOffset,-transform.up*range);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAdjustFixedToWorldVec)
        {
            RaycastHit hit;
            if(Physics.Raycast(parent.position+parentOriginOffset,-parent.up,out hit,range,groundMask))
            {
                transform.position = hit.point+offset;
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x,fixWorldVec.y,transform.position.z);
        }
    }
}
