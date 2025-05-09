using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [SerializeField] private float attackRange = 2.2f; //how close to start attack loop
    [SerializeField] private float attackStop = 2.5f; //how far to break out of the attack loop

    protected override void TickPatrol() 
    {
        if (!agent.hasPath) //if the enemy doesnt have a path, set destination to the next patrol point
        {
            agent.SetDestination(GetNextPatrolPoint());
        }
        if (Target) // if the enemy finds a target, change to chasing
        {
            ChangeState(EnemyState.Chasing);
        }
    }

    protected override void TickChase()
    {
        if (!Target) { // if the enemy loses it's target, go back to patrolling
            ChangeState(EnemyState.Patrolling);
            return; 
        }

        float d = Vector3.Distance(transform.position, Target.position); // get distance from enemy to player
        agent.SetDestination(Target.position); // set destination to player

        if (d <= attackRange) ChangeState(EnemyState.Attacking); // if within range of player, switch to attack
    }

    protected override void TickAttack()
    {
        if (!Target) { // also allow switching back to patrol from attacking if no target
            ChangeState(EnemyState.Patrolling);
            return;
        }

        transform.LookAt(Target); //make the enemy look towards the player(may have to slow this down to make it natural)

        // pick next attack and trigger the animation for that attack
        AttackData atk = GetNextAttack();
        anim.SetTrigger(atk.animTrigger);

        ChangeState(Vector3.Distance(transform.position, Target.position) > attackStop // go back to chasing if the player gets too far away
                    ? EnemyState.Chasing : EnemyState.Attacking);
    }

    // ping pong patrol system
    private int patrolIndex, dir = 1; //go through children of enemy, looking for patroll points
    private Vector3 GetNextPatrolPoint()
    {
        var p = transform.GetChild(patrolIndex).position;
        patrolIndex += dir;
        if (patrolIndex == transform.childCount || patrolIndex < 0)
        { dir *= -1; patrolIndex += dir; }   // reverse
        return p;
    }
}
