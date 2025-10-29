using UnityEngine;

[CreateAssetMenu]
public class EnemyDeathStateBase : StateBase
{
    public override void OnEnter()
    {
        Debug.Log($"Ž€–S,{owner.owerObj.name}");
        UnityEngine.Object.Destroy(owner.owerObj);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}
