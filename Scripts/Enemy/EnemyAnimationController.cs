using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private Enemy enemy;
    private Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<Enemy>();
        enemy.OnAttack += attackAnimation;
      //  player.onDeath += idleAnimation;
        
    }
    private void Update()
    {
        if(player == null)
        {
            Enemy.PlayerIsDeath = true;
        }
        idleAnimation();
    }
    private void attackAnimation()
    {
        animator.SetTrigger("Attack");
    }
    private void idleAnimation()
    {
        if (Enemy.PlayerIsDeath == true)
        {
            animator.SetBool("isIdle", true);
        }        
    }
}
