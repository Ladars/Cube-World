using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : LivingEntity
{
    public static event System.Action OnDeathStatic;
    public event System.Action OnAttack;
    public enum State { Idle,Chasing,Attacking,RobotAttack};
    protected State currentState;
    public enum EnemyType { Box,Sphere};
    public EnemyType enemyType;
    protected NavMeshAgent pathFinder;
    protected Transform target;
    [Header("Attack")]
    //attack variables
    public float attackDistanceThreshold = 1.5f;
    public float attackInterpolation;
    public int damage;
    float timeBetweenAttacks = 1;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    public Material skinMaterial;
    Color originalColor;
    BoxCollider boxCollider;
    //Death variables
   
    //LivingEntity targetEntity;
    protected Player targetEntity;
    protected bool hasTarget;
    public static bool PlayerIsDeath;
    //public ParticleSystem hurtEffect;
    public AudioSource audioSource;
    public AudioClip deathSfx;
    protected virtual void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();

        boxCollider = GetComponent<BoxCollider>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {

            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<Player>();
            if(enemyType == EnemyType.Sphere)
            {
                myCollisionRadius = GetComponent<CapsuleCollider>().radius;
                targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            }
            if(enemyType == EnemyType.Box)
            {
                myCollisionRadius = GetComponent<BoxCollider>().size.x/2;
                targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            }
        }
        }
    protected override void Start()
    {
        base.Start();
        PlayerIsDeath = false;
        boxCollider = GetComponent<BoxCollider>();
        if(hasTarget)
        {
            currentState = State.Chasing;
            Move();
            targetEntity.onDeath += OntargetDeath;
        }
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))//
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    Attack();
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                }
            }
        }        
    }
    protected virtual void Move()
    {
        StartCoroutine(UpdatePath());
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)//敌人被击中的方法
    {
        //AudioManager.instance.PlaySound("Impact", transform.position);
 
        if (damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("EnemyDeath", target.transform.position);
            audioSource.PlayOneShot(deathSfx, 1);

            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection))as GameObject,2f);
        }
        else
        {
            Destroy(Instantiate(hurtEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, 2f);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }
    //IEnumerator Attack()
    //{
    //    currentState = State.Attacking;
    //    pathFinder.enabled = false;
    //    Vector3 originalPosition = transform.position;
    //    Vector3 dirToTarget = (target.position - transform.position).normalized;
    //    Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);//使敌人与玩家保持一定距离
    //    float percent = 0;
    //    float attackSpeed = 3;
    //    if(percent <= 1)
    //    {
    //        //Debug.Log("true");
    //        percent += Time.deltaTime*attackSpeed;
    //        float interpolation = (-Mathf.Pow(percent,2) * percent+percent)*4;
    //        transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
    //        yield return new WaitForSeconds(0.2f);
    //    }
    //    currentState = State.Chasing;
    //    pathFinder.enabled = true;
    //}
    void Attack()
    {
        if (OnAttack !=null)
        {
            OnAttack();
        }
        currentState = State.Attacking;
        pathFinder.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius*0.1f);//使敌人与玩家保持一定距离
        //StartCoroutine(colorChange());
        //StartCoroutine(attckInvoke());
        //bool hasAppliedDamage = false;
        transform.position = Vector3.Lerp(originalPosition, target.position, attackInterpolation);
        //hasAppliedDamage = true;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }
    void OntargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.5f;
       
            while (hasTarget)
            {
                if (currentState == State.Chasing)
                {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius*2 + targetCollisionRadius);//使敌人与玩家保持一定距离
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                    yield return new WaitForSeconds(0.5f);
                }
               
                yield return new WaitForSeconds(refreshRate);
                 }
             }
        if (hasTarget==false)
        {
            pathFinder.SetDestination(transform.position);
        }
    }
    IEnumerator colorChange()//敌人攻击时，其自身颜色会发生改变
    {
        skinMaterial.color = Color.blue;
        yield return new WaitForSeconds(0.3f);
        skinMaterial.color = originalColor;
    }
    #region
    //IEnumerator attckInvoke()
    //{
    //    yield return new WaitForSeconds(0.05f);
    //    boxCollider.enabled = false;
    //    yield return new WaitForSeconds(0.1f);
    //    boxCollider.enabled = false;
    //}
    #endregion
    [System.Obsolete]
    public void SetCharacteristics(float moveSpeed,int hitsToKillPlayer,float enemyHealth)//使不同关卡的敌人有不同的血量和颜色 
    {                                                                                                                                                                     //make different levels of enemy with different health and color
        pathFinder.speed = moveSpeed;
        if (hasTarget)
        {
            damage = (int)Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;
       // hurtEffect.startColor = new Color(skinColour.r, skinColour.g, skinColour.b, 1);//使敌人击中特效颜色与敌人颜色相同
       // deathEffect.startColor = new Color(skinColour.r, skinColour.g, skinColour.b, 1); //Make the enemy hit effect color the same as the enemy color
        skinMaterial = GetComponentInChildren<Renderer>().material;
       // skinMaterial.color = skinColour;
       // originalColor = skinMaterial.color;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            targetEntity.TakeDamage(damage);
            targetEntity.HurtEffect();
            Vector3 direction = -(transform.position - targetEntity.transform.position).normalized*15;
           // Debug.Log(direction);
            targetEntity.kockBack(direction);
        }
    }
}
