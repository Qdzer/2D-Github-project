using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeePatrolStare : BaseState
{
    private Vector3 target;
    private Vector3 moveDir;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        target = enemy.GetNewPotion();
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
            currentEnemy.SwichState(NPCState.Chase);

        if(Mathf.Abs(target.x - currentEnemy.transform.position.x)<0.1f  &&  Mathf.Abs(target.y - currentEnemy.transform.position.y) < 0.1f)
        {
            currentEnemy.wait = true;
            target = currentEnemy.GetNewPotion();
        }

        moveDir = (target - currentEnemy.transform.position).normalized;

            if (moveDir.x > 0)
                currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
            if (moveDir.x < 0)
            {
                currentEnemy.transform.localScale = new Vector3(1, 1, 1);
            }

        }
    

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isDead && !currentEnemy.isHurt && !currentEnemy.wait)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }
    public override void OnExit()
    {
        
    }
}
