using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Gun
{
    [Header("Laser")]
    public Transform LaserShootPoint;
    [SerializeField] LineRenderer laserLine;
    public float laserTime = 0;
    public float laserDamageInterval = 0.1f;
    private void Update()
    {
        if (laserTime > 0)
        {
            laserTime -= Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0))
        {
            laserLine.enabled = false;
        }
    }
    protected override void shoot()
    {
        base.shoot();
        if (fireMode == FireMode.Burst)
        {
            Debug.Log("1");
            laserLine.SetPosition(0, LaserShootPoint.position);
            RaycastHit hit;
            if (Physics.Raycast(LaserShootPoint.position, transform.forward, out hit))
            {
                laserLine.SetPosition(1, hit.point);
                if (hit.collider.CompareTag("Enemy"))
                {
                    laserLine.enabled = true;
                    if (laserTime <= 0)
                    {
                        laserTime += laserDamageInterval;
                    }
                }
            }
            else
            {
                laserLine.SetPosition(1, LaserShootPoint.position + transform.forward * 50);
            }
        }      
    }
   
}
