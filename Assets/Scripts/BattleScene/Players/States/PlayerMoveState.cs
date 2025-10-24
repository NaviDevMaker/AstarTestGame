using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Player
{
    public class PlayerMoveState : StateMachineBase<PlayerController>
    {
        public PlayerMoveState(PlayerController controller) : base(controller)
        {
            Initialize();
        }
        float moveSpeed = 0f;
        enum PressedKey
        {
            Right,
            Left,
            Foward,
            Back
        }

        readonly Dictionary<PressedKey, Vector3> directionDic = new Dictionary<PressedKey, Vector3>
        {
            {PressedKey.Foward,Vector3.forward},
            {PressedKey.Back,Vector3.back},
            {PressedKey.Right,Vector3.right},
            {PressedKey.Left,Vector3.left}
        };

        public override void OnEnter()
        {
            nextState = controller.PlayerIdleState;
        }
        public override void OnExit()
        {
        }
        public override void OnUpdate()
        {
            var key = GetKey();
            Debug.Log(default(KeyCode));
            if (key == default) return;
            var direction = directionDic[key];
            Move(direction);
        }
        void Move(Vector3 direction)
        {
            var currentPos = controller.transform.position;
            var targetPos = currentPos + direction * moveSpeed * Time.deltaTime;
            var move = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
            controller.transform.position = move;
        }
        PressedKey GetKey()
        {
            if (Input.GetKey(KeyCode.W)) return PressedKey.Foward;
            if (Input.GetKey(KeyCode.S)) return PressedKey.Back;
            if (Input.GetKey(KeyCode.A)) return PressedKey.Left;
            if (Input.GetKey(KeyCode.D)) return PressedKey.Right;
            return default;
            //var current = default(KeyCode);
            //foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            //{
            //    if (Input.GetKey(keyCode))
            //    {
            //        current = keyCode;
            //        break;
            //    }
            //}

            //return current switch
            //{
            //    KeyCode.W => PressedKey.Foward,
            //    KeyCode.S => PressedKey.Back,
            //    KeyCode.A => PressedKey.Left,
            //    KeyCode.D => PressedKey.Right,
            //    _ => default,
            //};
        }
        void Initialize()
        {
            moveSpeed = controller.PlayerStatusData.MoveSpeed;

        }
    }
}

