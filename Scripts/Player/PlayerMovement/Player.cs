using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

[RequireComponent(typeof(PlayerController)) ]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity//用于玩家的控制
{
    private CapsuleCollider capsuleCollider;
    public float moveSpeed = 5;
    PlayerController controller;
    Camera viewCamera;
    GunController gunController;
    public CrossHair crossHair;//准心
    public UIDoTween uIDoTween;
    //gunChange
    public Action<int> onGunChange;
    [HideInInspector]public float GuncooldownTimeCount;
    [HideInInspector] public float FlashcooldownTimeCount;
    public bool gunCanChange;
    //Flash
    public Action onFlashUse;
    public bool FlashCanUse;
    public bool canTeleport;
    Ray ray;
    private enum GunType { raffle,pistol };
    GunType currentGunType;
    public LayerMask layerMask; 
    [HideInInspector] public Vector3 moveInput;

    //particle Effect
    public ParticleSystem TeleportEffect;
    public ParticleSystem playerHurtEffect;
    //Rigibody
    Rigidbody rigidbody;
    //Dash
    float dashTime=0;
    Vector3 moveVelocity;
    public bool canDash;
    public Action onDashUse;
    public AudioClip DashAudio;
    [Header("Invisible var")]
    public float invincibilityDuration = 1f; // Duration of invincibility in seconds
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dashSFX;
    public AudioClip hurtSFX;
    public AudioClip teleportSFX;
    private void Awake()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        gunController = GetComponent<GunController>();
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    protected override  void Start()
    {
        uIDoTween = FindObjectOfType<UIDoTween>();
        gunController.EquipGun(1);
        base.Start();
        //controller = GetComponent<PlayerController>();
        //viewCamera = Camera.main;
        //gunController = GetComponent<GunController>();
    }
    private void CooldownTime()
    {
        if (GuncooldownTimeCount > 0)
        {
            gunCanChange = false;
           
            GuncooldownTimeCount -= Time.deltaTime;
        }
        else
        {
            gunCanChange = true;
            FlashCanUse = true;
        }
        if(FlashcooldownTimeCount > 0)
        {
            FlashCanUse = false;
            FlashcooldownTimeCount -= Time.deltaTime;
        }
        else
        {
            FlashCanUse = true;
        }
    }
    private void Update()
    {
        CooldownTime();
        PlayerMove();
        PlayerLook();
        
        if (Input.GetKeyDown(KeyCode.Alpha1)&&gunCanChange==true&&currentGunType!=GunType.pistol)
        {
            currentGunType = GunType.pistol;
            GuncooldownTimeCount = 5;
            gunController.EquipGun(0);
            if(onGunChange != null)
            {
                onGunChange(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && gunCanChange == true && currentGunType != GunType.raffle)
        {
            currentGunType = GunType.raffle;
            GuncooldownTimeCount = 5;
            gunController.EquipGun(1);
            if (onGunChange != null)
            {
                onGunChange(1);
            }
        }

        //weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OntriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
        //player die
        if (transform.position.y < -10)
        {
            TakeDamage(startingHealth);
        }
        Dash();
    }
    
    void Dash()
    {
        Vector3 worldPos = viewCamera.ScreenToWorldPoint(Input.mousePosition);
        float dotProduct = Vector3.Dot(transform.forward, moveInput);
        bool isMovingForward = dotProduct > 0;
        if (dashTime>0)
        {
            dashTime -= Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(1)&& dashTime <= 0&&canDash ==true&&isMovingForward)
        {
            onDashUse();
            audioSource.PlayOneShot(dashSFX);
          //  AudioManager.instance.PlaySound(DashAudio, transform.position);
            dashTime = 1;
            StartCoroutine(InvincibilityCoroutine(invincibilityDuration));
            //rigidbody.DOMove(transform.position + (transform.forward * 10), 0.5f);
            //rigidbody.AddForce(transform.forward * 15, ForceMode.Impulse);
            rigidbody.velocity = transform.forward*15;
            Invoke("ResetDash", 0.5f);
        }    
    }
    void ResetDash()
    {
        rigidbody.velocity = Vector3.zero;
    }
    void OnNewWave(int waveNumber)
    {
        health = startingHealth;

    }
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
       
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, 2f);
        }
        else
        {
            audioSource.PlayOneShot(hurtSFX);
            Destroy(Instantiate(playerHurtEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, 2f);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }
    private void PlayerMove()
    {
         moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);
    }
    private void PlayerLook()
    {
        ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up*gunController.GunHeight());
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
            crossHair.transform.position = point;//使准心位置与鼠标位置一致 Align the crossHair position with the mouse position
            crossHair.DetectTargets(ray);
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).magnitude > 2)
            {
                gunController.Aim(point);
            }           
                Teleport(point);                    
        }
    }
    private void Teleport(Vector3 point)//传送玩家
    {
        if (Input.GetKey(KeyCode.F)&&FlashCanUse == true)
        {
            canDash = false;
            uIDoTween.FlashAlphaEffect(0.5f);
            Time.timeScale = 0.1f;
            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo,999f, layerMask))
            {
                canTeleport = true;
                Debug.DrawLine(ray.origin,hitInfo.point, Color.red);
                if (Input.GetMouseButton(1))//传送前判断障碍物，如果检测到障碍物则不能传送
                {
                    audioSource.PlayOneShot(teleportSFX);
                    uIDoTween.GunDoShakeEffect();
                    FlashcooldownTimeCount = 10;
                    Destroy(Instantiate(TeleportEffect.gameObject, point, Quaternion.identity) as GameObject,2f);
                    Vector3 mouseWordPositon = Camera.main.ScreenToWorldPoint(point);
                    gameObject.transform.position = point;
                    if (onFlashUse != null)
                    {
                        onFlashUse();
                    }
                    Time.timeScale = 1;
                }
            }
            else
            {
                Debug.Log("detect false");
                canTeleport = false;
            }
            
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            canDash = true;
            Time.timeScale = 1f;
            uIDoTween.FlashAlphaEffect(0);
            FlashcooldownTimeCount = 10;
        }

    }
    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death",transform.position);
        Destroy(Instantiate(deathEffect.gameObject, transform.position, Quaternion.identity) as GameObject, 2);
        Enemy.PlayerIsDeath = true;
        base.Die();
    }
    public void kockBack(Vector3 KockBackDirection)
    {
        rigidbody.AddForce(KockBackDirection, ForceMode.Impulse);
        StartCoroutine(KockBackCoroutine());
    }
    public IEnumerator KockBackCoroutine()
    {    
        yield return new WaitForSeconds(0.3f);
        rigidbody.velocity = Vector3.zero;
    }
    //IEnumerator InvincibilityCoroutine()
    //{
    //    isInvincible = false;
    //    yield return new WaitForSeconds(invincibilityDuration);
    //    isInvincible = true;

    //}
}
