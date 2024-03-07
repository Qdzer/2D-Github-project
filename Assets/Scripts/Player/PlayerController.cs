using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public PhysicsCheck physicsCheck;
    private Collider2D coll;
    public PlayerInputContorl inputControl;
    private PlayAnimation playerAnimation;
    private Rigidbody2D rb;
    private Animator anmi;
    private Character character;
    [Header("基本参数")]
    public Vector2 inputDirection;
    public float speed;
    private float runSpeed;
    public float hurtForce;
    public float jumpForce;
    public float jumpWall;
    public float slideDistance;
    public float slideSpeed;
    public int sildePowerCost;

    private float walkSpeed => speed / 2.5f;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")]
    public bool isAttack;

    public bool isHurt;

    public bool isDead;

    public bool wallJump;

    public bool jumping;

    public bool isSlide;
    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayAnimation>();
       anmi = GetComponent<Animator>();
        inputControl = new PlayerInputContorl();
        character = GetComponent<Character>();
        //跳跃
        inputControl.Gameplay.Jump.started += jump;
        #region 强制走路
        runSpeed = speed;
    inputControl.Gameplay.WakeButton.performed += ContextBoundObject=>
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };
        inputControl.Gameplay.WakeButton.canceled += ContextBoundObject => 
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion
        //攻击
        inputControl.Gameplay.Attcak.started += PlayerAttack;
        //滑铲
        inputControl.Gameplay.Slide.started += Slide;
    }


    private void OnEnable() 
    {
        inputControl.Enable();
    }

    private void OnDisable() 
    {
        inputControl.Disable();
    }
    private void Update() {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();

        
    }
    private void FixedUpdate()
    {
        if (!isHurt&&!isAttack)
         Move();

    }
  
 
    public void Move()
    {   //人物移动
        if(!wallJump)
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);
        //人物反转
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;
        transform.localScale = new Vector3(faceDir,1,1);
       
    }
    
    public void jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            isSlide = false;
            StopAllCoroutines();
            GetComponent<AudioDefination>()?.PlaAudioClip();
        }

      
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2f) * jumpWall, ForceMode2D.Impulse);
            wallJump = true;
        }

    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            playerAnimation.PlayerAttack();
            isAttack = true;
        }
    }


    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround&&character.currentPower >= sildePowerCost)
        {
            isSlide = true;
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggSlide(targetPos));

            character.OnSlide(sildePowerCost);
        }


    }

    private IEnumerator TriggSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)
                break;
            if (physicsCheck.touchLeftWall&&transform.localScale.x<0f || physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x +transform.localScale.x*slideSpeed, transform.position.y));
        }
        while (MathF.Abs(target.x-transform.position.x)>0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }
    public void PlaterDead() 
    {
        isDead = true;
        inputControl.Gameplay.Disable();
        this.gameObject.layer =0;
    }
    #endregion
    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if (physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if (wallJump && rb.velocity.y < 0f)
            wallJump = false;

        if (isDead || isSlide)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }

    
}



