using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Player player;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();
        player.onDashUse = DashAnimate;
    }
    private void Update()
    {
        MoveAnimation();
    }
    public void MoveAnimation()
    {
        animator.SetFloat("MoveX", player.moveInput.x);
        animator.SetFloat("MoveZ", player.moveInput.z);
    }
    public void DashAnimate()
    {
        animator.SetTrigger("IsDash");
    }
}
