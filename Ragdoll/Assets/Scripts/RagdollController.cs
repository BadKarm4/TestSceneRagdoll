using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Rigidbody targetLimb;
    public Rigidbody hips;
    public bool mirror;
    public Rigidbody r;
    private ConfigurableJoint cj;

    private void Start()
    {
        //cj = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        if (!mirror)
        {
            targetLimb.MoveRotation(hips.rotation);
            //targetLimb.rotation = hips.rotation; 
            //cj.targetRotation = targetLimb.rotation;
        }
        else
        {
            //cj.targetRotation = Quaternion.Inverse(targetLimb.rotation);
        }
    }
}
