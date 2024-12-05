using UnityEngine;

public class OnAttackState : IEnemyState
{
    public IEnemyState HandleState(OffensiveEnemyController enemy)
    {
        IEnemyState toSwitch = enemy.CurrentState;


        return toSwitch;
    }

    public IEnemyState ChangeState()
    {


        return null;
    }

}
