using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gun : MonoBehaviour
{
    public Transform[] muzzle;
    public Projectile projectile;
    public float shootInterval=100;
    public float muzzleVelocity = 35;
    float nextShotTime;
    MuzzleFlash muzzleFlash;
    //weapon type
    public enum FireMode { Auto,Burst,Single};
    public FireMode fireMode;
    //single Shoot var
    bool triggerReleasedSinceLastShot;
    //burst Shoot var
    public int burstCount;
    int shotsRemainingInBurst;
    //recoil var 
    Vector3 recoilSmoothDampVelocity;
    float recoilAngle;
    float recoilRotSmoothDampVelocity;
    //reload var
    public int projectilesPerMag;
    public int projectilesRemainingInMag;
    public bool isReloading;
    public float reloadTime = 0.3f;
    [Header("Recoil")]
    public Vector2 kickMinMax=new Vector2(0.05f,.2f);
    public Vector2 recoilAngleMinMax=new Vector2(10,15);
    public float recoilMoveSettleTime=.1f;//横向归位时间
    public float recoilRotationSettleTime=.1f;//纵向归位时间
    //弹壳 Shell
    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    public event System.Action onReload;
  
    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }
    private void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle,0,ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;//使枪射击时向上进行少量的偏移 offsets the gun upward by a small amount when firing
        if(!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }
  
    protected virtual void shoot()//射击方法
    {
        if(!isReloading&&Time.time > nextShotTime&&projectilesRemainingInMag>0)
        {
            if(fireMode == FireMode.Burst)
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            else if (fireMode == FireMode.Auto)
            {
               
            }
            //if (fireMode != FireMode.Auto)
            //{
              
            //}
            for (int i = 0; i < muzzle.Length; i++)
            {
                if (projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + shootInterval / 1000;
                Projectile newProjectile = Instantiate(projectile, muzzle[i].position, muzzle[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * UnityEngine.Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += UnityEngine.Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }       
    }
    public void Reload()//上弹
    {
        if(!isReloading&& projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimaReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
            if (onReload != null)
            {
                onReload();
            }
        }      
    }
    IEnumerator AnimaReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);
        float reloadSpeed = 1 / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30; 
        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) * percent + percent) * 4;
            float reloadAngle = Mathf.Lerp(0,maxReloadAngle,interpolation);
           
            transform.localEulerAngles = initialRot + Vector3.left *reloadAngle;
            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }
    public void Aim(Vector3 aimPoint)//瞄准点
    {    
            transform.LookAt(aimPoint);            
    }
    public void OntriggerHold()
    {
        shoot();
        triggerReleasedSinceLastShot = false;
    }
    public void OntriggerRelease()
    {
        
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
  
}
