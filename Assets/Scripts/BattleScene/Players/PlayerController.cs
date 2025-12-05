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
        (int hash,string clipName,float length,float stateSpeed) GetAnimInfo(PlayerStateMachineBase<TPlayer> stateMachineBase);
        UnityAction OnHitEnemyAction { get; set;}
        Func<float,UniTask> OnDeadAction { get; set;}
        Animator animator { get; }
        Func<float,UniTask> OnAttackingAction { get; set;}
        UnityAction<IEnemy> OnKillEnemyAction { get; set;}
        bool isDead { get;}
        bool isInvincible { get; set;}
        List<Material[]> meshMats { get;}
        static TPlayer instance { get;}//interface‚àstatic‚¢‚¯‚é‚ç‚µ‚¢
        PlayerAudioHelper audioHelper { get;}
        int enemyDestroyCount { get;set;}

        UnityAction OnInvokedSpecialMove { get; set;}
     }
    public class PlayerController : MonoBehaviour,IAssetSetter,IPlayer<PlayerController>,IDamageable,ISetUper
    {
        public AnimationData animationData { get; private set; }
        public PlayerIdleState _playerIdleState { get; private set;}
        public PlayerWalkState _playerWalkState { get; private set;}
        public PlayerAttackState _playerAttackState { get; private set; }
        public PlayerHitState _playerHitState { get; private set; }
        public PlayerDeathState _playerDeathState { get; private set;}
        public PlayerSpecialMoveState _playerSpecialMoveState {  get; private set; }
        public PlayerItemPickUpState _playerItemPickUpState { get; private set;}
        PlayerStateMachineBase<PlayerController> currentState = null;

        [SerializeField] AudioSources audioSources;
        [SerializeField] PlayerStatusData statusData;
        [SerializeField] PlayerAudioDatas audioDatas;
        [SerializeField] PlayerTweenFieldDatas tweenFieldDatas;
        public PlayerStatusData playerStatusData => statusData;
        public PlayerTweenFieldDatas playerTweenFieldDatas => tweenFieldDatas;
        public Animator animator { get; private set; }
        public bool isDead { get; private set; }
        public IEnemy currentTarget { get; set;}
        public HashSet<IEnemy> specialMoveTargets { get; set; } = new HashSet<IEnemy>();
        public UnityAction OnHitEnemyAction { get; set;} 
        public int currentLife { get; set;}
        public bool isInvincible { get; set; }
        public Func<float,UniTask>OnDeadAction { get; set; }
        public List<Material[]> meshMats { get; private set; } = new List<Material[]>();
        public UnityAction<IEnemy> OnKillEnemyAction { get; set; }
        public PlayerAudioDatas AudioDatas => audioDatas;
        public PlayerAudioHelper audioHelper { get; private set; }

        public static PlayerController instance { get; private set; }
        public Func<float, UniTask> OnAttackingAction { get; set; }
        public bool IsSetUped { get; set; } = false;
        public int enemyDestroyCount { get; set; }
        public UnityAction OnInvokedSpecialMove { get; set; }

        [Serializable]
        class AudioSources
        {
            [SerializeField] AudioSource sfxAudioSource;
            [SerializeField] AudioSource footAudioSource;

            public AudioSource SfxAudioSource => sfxAudioSource;
            public AudioSource FootAudioSource  => footAudioSource;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await GetAsset();
            Initialize();
        }
        // Update is called once per frame
        void Update()
        {
            if (!IsSetUped) return;
            Debug.Log(currentTarget);
            if (isDead && currentState != _playerDeathState)
            {
                DeathAction();
                return;
            }
            if (isDead) return;
            if (InputManager.AttackButtonPressed()) _playerAttackState.Attack().Forget();
            if (InputManager.PickUpItemButtonPressed()) _playerItemPickUpState.TryPickUpItem();
            if(InputManager.SpecialMoveButtonPressed() 
                && enemyDestroyCount >= playerStatusData.SpecialMovableCount) _playerSpecialMoveState.SpecialMove().Forget();
            currentState?.OnUpdate();
        }
        void Initialize()
        {
            animator = GetComponent<Animator>();
            var sfxAudioSource = audioSources.SfxAudioSource;
            var footAudioSource = audioSources.FootAudioSource;
            audioHelper = new PlayerAudioHelper(audioDatas,sfxAudioSource,footAudioSource);
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
            _playerSpecialMoveState = new PlayerSpecialMoveState(this);
        }
        void PlayerSetUp()
        {
            instance = this;
            currentLife = playerStatusData.Life;
            OnHitEnemyAction += _playerHitState.WaitInvincibleTime;
            OnHitEnemyAction += () =>
            {
                if (isDead) return;
                audioHelper.PlayHittedAudio();
            };
                
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
        public (int hash,string clipName
              ,float length,float stateSpeed) GetAnimInfo(PlayerStateMachineBase<PlayerController> stateMachineBase)
        {
            return stateMachineBase switch
            {
                PlayerWalkState => (animationData.WalkHash, animationData.WalkClipName
                                   , animator.GetControllerLength(animationData.WalkClipName)
                                   , animator.GetStateSpeed(animationData.WalkClipName)),
                PlayerAttackState => (animationData.AttackHash, animationData.AttackClipName
                                     , animator.GetControllerLength(animationData.AttackClipName)
                                     , animator.GetStateSpeed(animationData.AttackClipName)),
                PlayerItemPickUpState => (animationData.PickUpHash, animationData.PickUpClipName
                                          , animator.GetControllerLength(animationData.PickUpClipName)
                                          , animator.GetStateSpeed(animationData.PickUpClipName)),
                PlayerDeathState => (animationData.DeathHash, animationData.DeathClipName
                                    , animator.GetControllerLength(animationData.DeathClipName)
                                    , animator.GetStateSpeed(animationData.DeathClipName)),
                PlayerSpecialMoveState => (animationData.SpecialMoveHash, animationData.SpecialMoveClipName
                                           , animator.GetControllerLength(animationData.SpecialMoveClipName)
                                           , animator.GetStateSpeed(animationData.SpecialMoveClipName)),
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
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, statusData.SpecialMoveRange);
        }
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            currentLife -= damage;
            LifeManager.Instance.ReduceLife(damage).Forget();
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


