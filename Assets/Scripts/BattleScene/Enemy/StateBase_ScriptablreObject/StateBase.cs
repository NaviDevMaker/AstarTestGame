using UnityEngine;

namespace Game.Enemy
{
    public abstract class StateBase : ScriptableObject
    {
        protected StateMachine stateMachine;
        protected IEnemy owner;
        protected Animator animator;
        public int animatorHash { get; protected set; }
        public virtual void Initialize(StateMachine stateMachine, IEnemy owner, Animator animator)
        {
            this.stateMachine = stateMachine;
            this.owner = owner;
            this.animator = animator;
        }

        public T Clone<T>() where T : StateBase => UnityEngine.Object.Instantiate(this) as T;

        public virtual void OnEnter() { OnEnterChangeAnimation(); }
        public abstract void OnUpdate();
        public virtual void OnExit() { OnExitChangeAnimation(); }

        public abstract void OnEnterChangeAnimation();
        public abstract void OnExitChangeAnimation();
    }
}


