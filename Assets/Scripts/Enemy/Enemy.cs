using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public Vector3 faceDir;
    public float HurtForce;
    public Transform attacker;
    public Vector3 spwanPoint;

    [Header("检查")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;
    public bool lost;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        //waitTimeCounter = waitTime;
        spwanPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();
        if(!isHurt&&!isDead&&!wait)
             Move();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("SnailPreMove") && !anim.GetCurrentAnimatorStateInfo(0).IsName("SnailRecover")&& !anim.GetCurrentAnimatorStateInfo(0).IsName("SnailDead"))
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }
    //计时器
    public void TimeCounter()
    {
       
            if (wait)
            {
                waitTimeCounter -= Time.deltaTime;
                if (waitTimeCounter <= 0)
                {
                    wait = false;
                    waitTimeCounter = waitTime;
                    transform.localScale = new Vector3(faceDir.x, 1, 1);
                }
            }
        if (!FoundPlayer()&&lostTimeCounter>0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
       
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);

    }
    
    public void SwichState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        } ;

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    public virtual Vector3 GetNewPotion()
    {
        return transform.position;
    }

    #region 事件执行方式
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //转身
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        //受伤击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 die = new Vector2(transform.position.x - attackTrans.position.x, 0);
        rb.velocity = new Vector2(0, rb.velocity.y);

        StartCoroutine(OnHurt(die));//启动协程
    }

    private IEnumerator OnHurt(Vector2 die)//迭代器
    {
        rb.AddForce(die * HurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }


    #endregion

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0), 0.3f);
    }
}
