using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageable
{
    public float startingHealth;
    public float health;// { get; protected set; }
    protected bool dead;
    [Header("Death")]
    public ParticleSystem hurtEffect;
    public ParticleSystem deathEffect;
    public event System.Action onDeath;
    public bool isInvincible = false; // Flag to track invincibility status
    protected virtual void  Start()
    {
        health = startingHealth;
    }
    public virtual void TakeHit(float damage,Vector3 hitPoint,Vector3 hitDirection)
    {
        TakeDamage(damage);
    }
    public virtual void TakeDamage(float damage)
    {
        if (isInvincible==true)
        {
            return;
        }
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    public virtual void HurtEffect()
    {
        if (isInvincible == true)
        {
            return;
        }
        Destroy(Instantiate(hurtEffect.gameObject, transform.position, Quaternion.identity) as GameObject, 2f);
    }
    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        dead = true;
        if(onDeath != null)
        {
            onDeath();
        }
        Destroy(gameObject);
    }
   protected  IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;

    }
}
