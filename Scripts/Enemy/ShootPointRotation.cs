using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPointRotation : MonoBehaviour
{
    Player target;
    private void Start()
    {
        target = FindAnyObjectByType<Player>();
    }
    private void Update()
    {
        LookAtPlayer();
    }
    void LookAtPlayer()
    {
       
            Vector3 relativePos = target.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = transform.localRotation;
            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime
               * 15);
     
    }
}
