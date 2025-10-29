using UnityEngine;
using UnityEngine.Events;

public interface IEnemyController
{
    Collider enemyCollider { get; }
    UnityAction OnDeadAction { get; }
    GameObject owerObj { get; }
}