using UnityEngine;
using System.Linq;
using Game.Item;
namespace Game.Player
{
    public class PlayerItemPickUpState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerItemPickUpState(PlayerController controller) : base(controller) { }

        float pickUpRadius = 0f;
        float pickUpAngle = 0f;//これは扇全体の角度
        public override void Initialize()
        {
            base.Initialize();
            pickUpRadius = controller.playerStatusData.PickUpRadius;
            pickUpAngle = controller.playerStatusData.PickUpAngle;
        }
        public override void OnEnter() { }
        public override void OnExit() { }
        public override void OnUpdate() { }

        public void TryPickUpItem()
        {
            var origin = controller.transform.position;
            var itemLayer = Layers.itemLayer;
            var hits = Physics.OverlapSphere(origin,pickUpRadius, itemLayer);
            var sortedHits = hits.ToList()
                             .OrderBy(hit => Vector3.Distance(hit.ClosestPoint(origin), origin))
                             .ToArray();
            sortedHits.ToList().ForEach(hit => Debug.Log($"ヒットアイテムの名前{hit.name}", hit.gameObject));
            var candidate = default(IPickupedItem);
            var minAngle = Mathf.Infinity;//PickUpAngleのなかのアイテムの中でさらに一番正面にあるアイテムのアングルを保存する、もしこれが
            foreach (var hit in sortedHits)
            {
                if (!hit.TryGetComponent<IPickupedItem>(out var pickupedItem)) continue;
                var closestPoint = hit.ClosestPoint(origin);
                if (!CanPickUpItem(closestPoint)) continue;
                var toTarget = closestPoint - origin;
                var forward = controller.transform.forward;
                toTarget.y = 0f;
                forward.y = 0f;
                var angle = Vector3.Angle(forward, toTarget);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    candidate = pickupedItem;
                }
            }

            if (candidate == null) return;
            candidate.OnPickUpItem(controller);
        }

        bool CanPickUpItem(Vector3 targetPos)
        {
            var forward = controller.transform.forward;
            var toTarget = (targetPos - controller.transform.position).normalized;
            forward.y = 0f;
            toTarget.y = 0f;
            var dot = Vector3.Dot(forward, toTarget);//中心線（この場合はfowardの方向の線）からtoTargetまでの内積の値
            var thereHold = Mathf.Cos(pickUpAngle * 0.5f * Mathf.Deg2Rad);//ここで0.5倍しないとpickupAngle由来のCosの値(全体の角度をもとにした値)とdot(中心線からの内積)を比べることになり不整合が起きる
            return dot >= thereHold;//threreHoldはpickUpAngleの値が上がれば上がるだけ値が上がり、dotは方向が同じだったら同じだけ値が大きくなるため、dot >= thereHoldとなる
        }
    }
}

