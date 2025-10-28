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
        protected string animationClipName { get; private set;}
        public TPlayer controller { get; private set; }
        public PlayerStateMachineBase<TPlayer> nextState { get; protected set; }
        public virtual void Initialize()
        {
            Debug.Log($"{this},{controller.GetAnimInfo(this)}");
            var info = controller.GetAnimInfo(this);
            this.animatorHash = info.hash;
            this.animationClipName = info.clipName;
        }
        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }

}

