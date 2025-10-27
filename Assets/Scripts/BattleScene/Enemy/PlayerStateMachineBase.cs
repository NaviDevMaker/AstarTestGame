using UnityEngine;

namespace Game.Player
{
    public abstract class PlayerStateMachineBase<TPlayer> where TPlayer : MonoBehaviour, IPlayer<TPlayer>
    {
        public PlayerStateMachineBase(TPlayer controller)
        {
            this.controller = controller;
            Initialize();
        }

        protected int animatorHash { get; private set; }
        public TPlayer controller { get; private set; }
        public PlayerStateMachineBase<TPlayer> nextState { get; protected set; }
        public virtual void Initialize()
        {
            Debug.Log($"{this},{controller.GetHash(this)}");
            this.animatorHash = controller.GetHash(this);
        }
        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }

}

