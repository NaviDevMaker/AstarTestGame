using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

using System.Linq;
using System;
using Game.Enemy;
using NUnit.Framework;

namespace Game.Player
{
    public interface IPlayer<TPlayer> : IDamageable where TPlayer : MonoBehaviour, IPlayer<TPlayer>
    {
        (int hash,string clipName,float length) GetAnimInfo(PlayerStateMachineBase<TPlayer> stateMachineBase);
        UnityAction OnHitEnemyAction { get; set;}
        Func<float,UniTask> OnDeadAction { get; set;}
        UnityAction<IEnemy> AddScoreAction { get; set;}
        bool isDead { get;}
        bool isInvincible { get; set;}
        List<Material[]> meshMats { get;}

        PlayerAudioHelper audioHelper { get;}
     }
    public class PlayerController : MonoBehaviour,IAssetSetter,IPlayer<PlayerController>,IDamageable
    {
        public AnimationData animationData { get; private set; }
        public PlayerIdleState _playerIdleState { get; private set;}
        public PlayerWalkState _playerWalkState { get; private set;}
        public PlayerAttackState _playerAttackState { get; private set; }
        public PlayerHitState _playerHitState { get; private set; }
        public PlayerDeathState _playerDeathState { get; private set;}

        public PlayerItemPickUpState _playerItemPickUpState { get; private set;}
        PlayerStateMachineBase<PlayerController> currentState = null;

        [SerializeField] AudioSource audioSource;
        [SerializeField] PlayerStatusData statusData;
        [SerializeField] PlayerAudioDatas audioDatas;
        [SerializeField] PlayerTweenFieldDatas tweenFieldDatas;
        public PlayerStatusData playerStatusData => statusData;
        public PlayerTweenFieldDatas playerTweenFieldDatas => tweenFieldDatas;
        public Animator animator { get; private set; }
        public bool isDead { get; private set; }
        public IEnemy currentTarget { get; set;}
        public UnityAction OnHitEnemyAction { get; set;} 
        public int currentLife { get; set;}
        public bool isInvincible { get; set; }
        public Func<float,UniTask>OnDeadAction { get; set; }
        public List<Material[]> meshMats { get; private set; } = new List<Material[]>();
        public UnityAction<IEnemy> AddScoreAction { get; set; }
        public PlayerAudioDatas AudioDatas => audioDatas;

        public PlayerAudioHelper audioHelper { get; private set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await GetAsset();
            Initialize();
        }
        // Update is called once per frame
        void Update()
        {
            Debug.Log(currentTarget);
            if (isDead && currentState != _playerDeathState)
            {
                DeathAction();
                return;
            }
            if (isDead) return;
            if (InputManager.AttackButtonPressed()) _playerAttackState.Attack().Forget();
            if (InputManager.PickUpItemButtonPressed()) _playerItemPickUpState.TryPickUpItem();
            currentState?.OnUpdate();
        }
        void Initialize()
        {
            animator = GetComponent<Animator>();
            audioHelper = new PlayerAudioHelper(audioDatas,audioSource);
            MaterialSetup();
            StateSetup();
            PlayerSetUp();
            ChangeState(_playerIdleState);
        }
        void StateSetup()
        {
            _playerIdleState = new PlayerIdleState(this);
            _playerWalkState = new PlayerWalkState(this);
            _playerAttackState = new PlayerAttackState(this);
            _playerHitState = new PlayerHitState(this);
            _playerDeathState = new PlayerDeathState(this);
            _playerItemPickUpState = new PlayerItemPickUpState(this);
        }
        void PlayerSetUp()
        {
            currentLife = playerStatusData.Life;
            OnHitEnemyAction += _playerHitState.WaitInvincibleTime;
            OnHitEnemyAction += audioHelper.PlayHittedAudio;
            OnDeadAction += async(_) =>
            {
                audioHelper.PlayDeathAudio();
                await UniTask.CompletedTask;
            };
        }
        public void ChangeState(PlayerStateMachineBase<PlayerController> nextState)
        {
            currentState?.OnExit();
            currentState = nextState;
            currentState.OnEnter();
        }
        public async UniTask GetAsset()
        {
            var animData = await GetAssetsMethods.GetAsset<AnimationData>("Datas/PlayerAnimationData");
            if (animData == null) throw new System.Exception();
            animationData = animData;
        }
        public (int hash,string clipName,float length) GetAnimInfo(PlayerStateMachineBase<PlayerController> stateMachineBase)
        {
            return stateMachineBase switch
            {
                PlayerWalkState => (animationData.WalkHash,animationData.WalkClipName,animator.GetControllerLength(animationData.WalkClipName)),
                PlayerAttackState => (animationData.AttackHash,animationData.AttackClipName,animator.GetControllerLength(animationData.AttackClipName)),
                PlayerItemPickUpState => (animationData.PickUpHash,animationData.PickUpClipName,animator.GetControllerLength(animationData.PickUpClipName)),
                PlayerDeathState => (animationData.DeathHash,animationData.DeathClipName,animator.GetControllerLength(animationData.DeathClipName)),
                _ => default
            };
        }
        void DeathAction() => ChangeState(_playerDeathState);
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, statusData.PickUpRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, statusData.DetectRange);
        }
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            currentLife -= damage;
            LifeManager.Instance.ReduceLife();
            if(currentLife <= 0) isDead = true;
        }
        public void SetHashToFalse() => _playerItemPickUpState.SetHashToFalse();

        void MaterialSetup()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                var rendererMats = renderer.materials;
                meshMats.Add(rendererMats);
            }
        }
    }
}


