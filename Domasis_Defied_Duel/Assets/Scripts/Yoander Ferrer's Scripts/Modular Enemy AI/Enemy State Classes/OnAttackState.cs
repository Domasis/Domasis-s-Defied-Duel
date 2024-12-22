using UnityEngine;
using UnityEngine.AI;

public class OnAttackState : IEnemyState
{
    public IEnemyState HandleState(OffensiveEnemyController enemy)
    {
        if (enemy.CanSeePlayer() && enemy.PlayerInRange)
        {
            enemy.IsAttacking = true;
            enemy.Agent.stoppingDistance = enemy.OriginalStoppingDistance;
            // We then set the NavMeshAgent's destination to the player's transform position.
            enemy.Agent.SetDestination(GameManager.instance.player.transform.position);

            // If the agent is within stopping distance:
            if (enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance)
            {
                // The AI will continuously turn towards the player in order to shoot them.
                enemy.FacePlayer();
            }

            // Finally, if the AI isn't shooting, it will call the Shoot() coroutine.
            if (!enemy.IsShooting)
            {
                enemy.StartCoroutine(enemy.Shoot());
            }

        }
        else if (!enemy.PlayerInRange)
        {
            enemy.Agent.stoppingDistance = 0;
            enemy.IsAttacking = false;

        }

        // We create an IEnemyState instance that will store the state our AI will switch to, defaulted to the current state of our enemy.
        IEnemyState stateToChange = enemy.CurrentState;
        if (!enemy.IsAttacking)
        {
            if (enemy.WasHit || enemy.HeardSomething)
            {

                // We set our state to our enemy's InvestigateState, which lets our AI know in Update to start calling logic from the OnInvestigateState class.
                stateToChange = enemy.InvestigateState;
            }

            // If our enemy can't see the player, and the player isn't in range:
            else if (!enemy.CanSeePlayer() && enemy.PlayerInRange)
            {

                // We set the AI's state to the RoamState, as we know that it's going to go back to roaming.
                stateToChange = enemy.RoamState;
            }

            else
            {
                stateToChange = enemy.RoamState;
            }
        }

        // Finally, regardless of the state of the enemy, we return it here so that our AI knows what state to be in for the next frame.
        return stateToChange;
    }


}