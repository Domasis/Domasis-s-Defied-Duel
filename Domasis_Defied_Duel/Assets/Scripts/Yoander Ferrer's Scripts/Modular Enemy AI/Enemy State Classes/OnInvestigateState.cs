using UnityEngine;

public class OnInvestigateState : IEnemyState
{
    public Coroutine investigate;

    public IEnemyState HandleState(OffensiveEnemyController enemy)
    {

        if (enemy.WasHit)
        {
            if (enemy.HeardSomething)
            {
                enemy.HeardSomething = false;
                if (investigate != null)
                {
                    enemy.StopCoroutine(investigate);
                    investigate = null;
                }
            }

                investigate = enemy.StartCoroutine(enemy.Investigate());

        }

        else if (enemy.HeardSomething)
        {
            if (!enemy.IsInvestigating)
            {
                investigate = enemy.StartCoroutine(enemy.Investigate());
            }
        }

        // We create an IEnemyState instance that will store the state our AI will switch to, defaulted to the current state of our enemy.
        IEnemyState stateToChange = enemy.CurrentState;

        // If our enemy can see the player, and the player is in range:
        if (enemy.CanSeePlayer() && enemy.PlayerInRange)
        {
            // We stop roaming in the same way we do above, nulling out our roam variable for the same reasons.
            enemy.IsInvestigating = 
            enemy.WasHit = 
            enemy.HeardSomething = false;

            if (investigate != null)
            {
                enemy.StopCoroutine(investigate);
                investigate = null;
            }

            // Finally, we set the AI's state to the AttackState, as we know that it's going to need to start attacking the player.
            stateToChange = enemy.AttackState;
        }

        else if (!enemy.CanSeePlayer() && enemy.PlayerInRange && !enemy.IsInvestigating)
        {
            stateToChange = enemy.RoamState;
        }

        // If our enemy can't see the player, and the player isn't in range:
        else if (!enemy.IsInvestigating)
        {

            // We set the AI's state to the RoamState, as we know that it's going to go back to roaming.
            stateToChange = enemy.RoamState;
        }

        // Finally, regardless of the state of the enemy, we return it here so that our AI knows what state to be in for the next frame.
        return stateToChange;
    }

}
