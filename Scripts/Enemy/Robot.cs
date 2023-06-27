using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Robot :Enemy
{
    public Image redImage;//"Red" Health bar Image
    public Image whiteImage;//"White Effect" Health bar Image
    [SerializeField] Projectile projectile;
    [SerializeField] private float hurtSpeed = 0.005f;
    //Robot Rotation toward Player
    [SerializeField] float rotationSpeed=5;
    //Robot Shoot
    public float desiredRange = 15f;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform shootingPoint1;
    [SerializeField] private float shootTimeInterval=1f;
    private float shootTime;
    private float reactionTime;
    public Player player;
    bool isReaction = false;
    [Header("Laser")]
    //Laser
    [SerializeField] LineRenderer laserLine;
    public Transform laserOrigin;
    private float laserDamageInterval=0.5f;
    private float laserTime;
    //Robot flame
    public GameObject flamePrefab;
    public float spawnRate = 2f;
    public float spawnRadius =25f;
    float flameSpawnInterval = 1;
    float spawnTimer;
    //animation
    Animator animator;
    enum RobotShootType { Bullet,Laser};
   [SerializeField]  RobotShootType shootType;
    private float TimeToChangeShootType=5;

    
    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        player.onFlashUse += LookAtPlayer;
        player.onFlashUse += Reaction;

    }
    private void healthBar()
    {
        redImage.fillAmount = health / startingHealth;  //制作掉血效果  红色的血条表示当前生命值，以当前生命值和最大生命值的比来显示

        if (whiteImage.fillAmount > redImage.fillAmount)
        {
            whiteImage.fillAmount -= hurtSpeed;
        }
        else
        {
            whiteImage.fillAmount = redImage.fillAmount;
        }
    }
    private void Reaction()
    {
        isReaction = true;
        reactionTime = 1.5f;
    }
    private void Update()
    {
       // healthBar();
        Move();
        if (health <=0)
        {
            GameObject bossHealthBar = GameObject.Find("BossHpBar");
            bossHealthBar.SetActive(false);
        }
       // RobotShootAttack();
        //RobotFlameAttack();
        LookAtPlayer();
    }
    private void RobotShootAttack()
    {
        animator.SetBool("IsWalk", false);

        laserLine.enabled = false;
        if (currentState == State.RobotAttack)
        {
            //if (Time.time > shootTime)
            //{
            // //   animator.SetTrigger("IsShoot");
              
            //}
            int angle = -40;
            int ratio;
            for (int i = 0; i < 5; i++)
            {
                ratio = i * 20;
                Projectile newProjectile = Instantiate(projectile, shootingPoint1.position, shootingPoint1.rotation * Quaternion.Euler(0, angle + ratio, 0));
                newProjectile.SetSpeed(35);
            }
            shootTime = Time.time + shootTimeInterval;
        }
    }
    private void RobotLaserAttack()
    {
        AudioManager.instance.PlaySound("RobotLaser", transform.position);
        laserLine.enabled = true;
        if (currentState == State.RobotAttack)
        {
            if (laserTime>=0)
            {
                laserTime -= Time.deltaTime;
            }           
            RaycastHit hit;
            laserLine.SetPosition(0, shootingPoint.position);
          //  Vector3 laserDirection = -(transform.position - player.transform.position);
            if (Physics.Raycast(shootingPoint.position,transform.forward,out hit))
            {
                laserLine.SetPosition(1,hit.point);
                if (hit.collider.CompareTag("Player"))
                {
                    if (laserTime <= 0)
                    {
                        laserTime += laserDamageInterval;
                        targetEntity.TakeDamage(10);
                        targetEntity.HurtEffect();
                        AudioManager.instance.PlaySound("EnemyDeath", target.transform.position);
                    }
                }             
            }
            else
            {
                laserLine.SetPosition(1, shootingPoint.position + transform.forward* 50);
            }
        }
    }
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            AudioManager.instance.PlaySound("RobotDeath", target.transform.position);
        }
        else
        {
            AudioManager.instance.PlaySound("Impact", target.transform.position);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }
    private void RobotFlameAttack()
    {
        spawnTimer += Time.deltaTime;
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
        if (spawnTimer>flameSpawnInterval)
        {
            Instantiate(flamePrefab, spawnPosition, Quaternion.identity);
            spawnTimer = 0;
        }
     
    }
    protected override void Move()
    {
        TimeToChangeShootType -= Time.deltaTime;
        currentState = State.RobotAttack;
        // float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (hasTarget)
        {
            Vector3 chaseDirection = -(transform.position - player.transform.position);
            RaycastHit hit;
            if (!Physics.Raycast(shootingPoint.position, chaseDirection, out hit, desiredRange) || !hit.collider.CompareTag("Player"))
            {
                pathFinder.SetDestination(target.position);
                laserLine.enabled = false;
                if (pathFinder.velocity.magnitude > 1f)
                {
                    animator.SetBool("IsWalk", true);
                }             
            }
            else
            {
                pathFinder.SetDestination(transform.position);
                animator.SetBool("IsWalk", false);
                if (TimeToChangeShootType <= 0)
                {
                    shootType = (RobotShootType)Random.Range(0, 2);
                    TimeToChangeShootType += 5;
                }
                if (shootType == RobotShootType.Laser)
                {
                    RobotLaserAttack();
                }
                else if (shootType == RobotShootType.Bullet)
                {
                    //RobotShootAttack();
                    animator.SetTrigger("IsShoot");
                }



            }
      
        }

        //if (distanceToPlayer > desiredRange)
        //{
        //    pathFinder.SetDestination(target.position);
        //    laserLine.enabled = false;
        //    animator.SetBool("IsWalk",true);
        //}
        //else
        //{
        //    pathFinder.SetDestination(transform.position);
        //    if (TimeToChangeShootType<=0)
        //    {
        //        shootType = (RobotShootType)Random.Range(0, 2);
        //        TimeToChangeShootType += 5;
        //    }
        //    if (shootType ==RobotShootType.Laser)
        //    {
        //        RobotLaserAttack();
        //    }
        //    else if (shootType ==RobotShootType.Bullet)
        //    {
        //        RobotShootAttack();
        //    }
            

        //    animator.SetBool("IsWalk", false);
        //}
          
    }
    void LookAtPlayer()
    {              
        if (isReaction==false)
        {
            Vector3 relativePos = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = transform.localRotation;
            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime
               * rotationSpeed);
        }
        else
        {
            reactionTime -= Time.deltaTime;
            if (reactionTime<=0)
            {
                isReaction = false;
            }
        }
    }
    void RobotSoundShootEvent()
    {
        AudioManager.instance.PlaySound("RobotShoot", target.transform.position);
    }
}
