using UnityEngine;
using System.Linq;
using Game.Item;
using Cysharp.Threading.Tasks;
namespace Game.Player
{
    public class PlayerItemPickUpState : PlayerStateMachineBase<PlayerController>,IAnimatorLayer
    {
        public PlayerItemPickUpState(PlayerController controller) : base(controller) { }

        float pickUpRadius = 0f;
        float pickUpAngle = 0f;//これは扇全体の角度
        bool isPicking = false;

        public int layerIndex { get; private set;}

        public override void Initialize()
        {
            base.Initialize();
            LayerSet();
            pickUpRadius = controller.playerStatusData.PickUpRadius;
            pickUpAngle = controller.playerStatusData.PickUpAngle;
        }
        public override void OnEnter() { }
        public override void OnExit() { }
        public override void OnUpdate() { }
        public void TryPickUpItem()
        {

            PlayPickItemAnimation();
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
                if (pickupedItem.isPicked) continue;
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
        void PlayPickItemAnimation()
        {
           if(!controller.animator.GetBool(animatorHash))  controller.animator.SetBool(animatorHash,true);
            controller.animator.Play(animationClipName,layerIndex,0f);
        }
        bool CanPickUpItem(Vector3 targetPos)
        {
            var forward = controller.transform.forward;
            var toTarget = targetPos - controller.transform.position;
            forward.y = 0f;
            forward.Normalize();
            toTarget.y = 0f;
            var distance = toTarget.magnitude;
            if (distance < 0.1f) return true;
            toTarget.Normalize();
            var dot = Vector3.Dot(forward, toTarget);//中心線（この場合はfowardの方向の線）からtoTargetまでの内積の値
            var thereHold = Mathf.Cos(pickUpAngle * 0.5f * Mathf.Deg2Rad);//ここで0.5倍しないとpickupAngle由来のCosの値(全体の角度をもとにした値)とdot(中心線からの内積)を比べることになり不整合が起きる
            Debug.Log($"dot:{dot},thereHold:{thereHold}");
            return dot >= thereHold;//threreHoldはpickUpAngleの値が上がれば上がるだけ値が上がり、dotは方向が同じだったら同じだけ値が大きくなるため、dot >= thereHoldとなる
        }
        public void SetHashToFalse() => controller.animator.SetBool(animatorHash, false);

        public void LayerSet() => layerIndex = controller.animationData.PickUpLayerIndex;
    }
}

