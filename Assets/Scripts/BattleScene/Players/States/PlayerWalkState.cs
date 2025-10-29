using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Player
{
    public class PlayerWalkState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerWalkState(PlayerController controller) : base(controller) { }
        float moveSpeed = 0f;
        float rotateSpeed = 0f;
        readonly Dictionary<PressedKey, Vector3> directionDic = new Dictionary<PressedKey, Vector3>
        {
            {PressedKey.Foward,Vector3.forward},
            {PressedKey.Back,Vector3.back},
            {PressedKey.Right,Vector3.right},
            {PressedKey.Left,Vector3.left}
        };
        int targetMask = -1;
        public override void OnEnter()
        {
            nextState = controller._playerIdleState;
        }
        public override void OnExit()
        {
            controller.animator.SetBool(animatorHash,false);
        }
        public override void OnUpdate()
        {
            var key = InputManager.GetKey();
            Debug.Log(key);
            if (key == PressedKey.None)
            {
                controller.ChangeState(nextState);
                return;
            }
            var direction = directionDic[key];
            if(!IsWalkable(direction))
            {
                controller.ChangeState(nextState);
                return;
            }
            Move(direction);
        }
        void Move(Vector3 direction)
        {
            if (!controller.animator.GetBool(animatorHash)) controller.animator.SetBool(animatorHash,true);
            var currentRot = controller.transform.rotation;
            var targetRot = Quaternion.LookRotation(direction);
            controller.transform.rotation = Quaternion.Slerp
                (
                   currentRot,
                   targetRot,
                   rotateSpeed * Time.deltaTime
                );
            var currentPos = controller.transform.position;
            var targetPos = currentPos + direction * moveSpeed * Time.deltaTime;
            var move = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
            controller.transform.position = move;
        }   
        public override void Initialize()
        {
            base.Initialize();
            moveSpeed = controller.playerStatusData.MoveSpeed;
            rotateSpeed = controller.playerStatusData.RotateSpeed;
            targetMask = Layers.enemyLayer | Layers.wallLayer;
        }
        bool IsWalkable(Vector3 direction)
        {
            var rayDistance = moveSpeed * Time.deltaTime;
            var pos = controller.transform.position;
           
            if(Physics.Raycast(pos, direction, rayDistance, targetMask)) return false;
            return true;
        }
    }
}


