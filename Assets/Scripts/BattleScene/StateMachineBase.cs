using UnityEngine;

public abstract class StateMachineBase<TOnwer> where TOnwer : MonoBehaviour
{
    public StateMachineBase(TOnwer controller)
    {
        this.controller = controller;
    }
    public TOnwer controller { get; private set;}
    public StateMachineBase<TOnwer> nextState { get; protected set;}
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
