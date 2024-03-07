using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    private Animator anmi;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;
    

    private void Awake()
    {
        anmi = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anmi.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        anmi.SetFloat("velocityY", rb.velocity.y);
        anmi.SetBool("isGround", physicsCheck.isGround);
        anmi.SetBool("isDead", playerController.isDead);
        anmi.SetBool("isAttack", playerController.isAttack);
        anmi.SetBool("onWall", physicsCheck.onWall);
        anmi.SetBool("isSlide", playerController.isSlide);
    }

    public void playerHurt()
    {
        anmi.SetTrigger("hurt");
    }

    public void PlayerAttack()
    {
        anmi.SetTrigger("attack");
    }
}
