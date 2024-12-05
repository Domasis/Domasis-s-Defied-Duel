using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// State Machine Subclass that inherits from our IEnemyState Interface and MonoBehaviour.
public class OnRoamState : IEnemyState
{
    // Coroutine variable that stores our current instance of our Roam coroutine.
    Coroutine roam;

    /* Reference to our enemy that can be called in class and helper methods.
     * This is required because we need that specific agent's stats, not just any agent's stats.*/
    OffensiveEnemyController enemyReference;

    // Interface method that performs the state actions, then determines what state needs to be invoked next.
    public IEnemyState HandleState(OffensiveEnemyController enemy)
    {
        // We set our enemyReference variable to the passed in enemy so that other methods can access its public member parameters.
        enemyReference = enemy;

        // If our enemy is roaming, we don't want to start another roam, so we only enter this if-statement if our AI isn't currently roaming.
        if (!enemy.IsRoaming)
        {
            // We set our Coroutine variable to a new instance of the Roam() contained by our enemyReference.
            roam = enemyReference.StartCoroutine(enemyReference.Roam());
        }

        // Finally, we call ChangeState, which returns an IEnemyState that our AI will change to.
        return ChangeState();
    }


    // Interface method that takes the current state of our AI instance, and switches states based on what booleans are currently set to true.
    public IEnemyState ChangeState()
    {
        // We create an IEnemyState instance that will store the state our AI will switch to, defaulted to the current state of our enemy.
        IEnemyState stateToChange = enemyReference.CurrentState;

        // If our enemy was hit, or if our enemy heard something:
      if (enemyReference.WasHit || enemyReference.HeardSomething)
        {
            // We set IsRoaming to false to ensure we can return to roaming after any other actions.
            enemyReference.IsRoaming = false;

            // We tell our enemy to stop roaming by calling StopCoroutine on the roam Coroutine variable.
            enemyReference.StopCoroutine(roam);

            // Finally, we null out our roam variable because we are stopping the coroutine before the coroutine naturally sets the variable to null.
            if (roam != null)
            {
                roam = null;
            }

            // Finally, we set our state to our enemy's InvestigateState, which lets our AI know in Update to start calling logic from the OnInvestigateState class.
            stateToChange = OffensiveEnemyController.InvestigateState;
        }

      // Otherwise, if our enemy can see the player, and the player is in range:
      else if(enemyReference.CanSeePlayer() && enemyReference.PlayerInRange)
        {
            // We stop roaming in the same way we do above, nulling out our roam variable for the same reasons.
            enemyReference.IsRoaming = false;
            enemyReference.StopCoroutine(roam);
            if (roam != null)
            {
                roam = null;
            }

            // Finally, we set the AI's state to the AttackState, as we know that it's going to need to start attacking the player.
            stateToChange = OffensiveEnemyController.AttackState;
        }

        // Finally, regardless of the state of the enemy, we return it here so that our AI knows what state to be in for the next frame.
        return stateToChange;
    }

}
