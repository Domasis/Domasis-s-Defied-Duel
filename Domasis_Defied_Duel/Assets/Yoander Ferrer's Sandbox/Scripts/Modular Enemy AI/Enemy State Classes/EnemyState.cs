using UnityEngine;

public interface IEnemyState
{
    IEnemyState HandleState(OffensiveEnemyController enemy);

}
