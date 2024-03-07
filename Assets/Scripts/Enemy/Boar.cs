using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
    }
}
