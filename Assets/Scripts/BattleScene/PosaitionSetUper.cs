using UnityEngine;
using Game.Player;
public class PosaitionSetUper : MonoBehaviour
{
    [SerializeField] CameraMover topViewCameraMover;
    Terrain terrain;
    public void Initialize(Transform playerTra)
    {
        terrain = Terrain.activeTerrain;
        SetPlayerPos(out var offset,playerTra);
        //SetTerrainPos(offset);
    }
    void SetPlayerPos(out Vector3 playerOffset,Transform playerTra)
    {
        playerOffset = default;

        var defaultPos = terrain.transform.position;
        var y = topViewCameraMover.transform.position.y;
        //var topViewTargetPos = defaultPos;
        //topViewTargetPos.y += y;
        //topViewCamera.transform.position = topViewTargetPos;
        //var terrainOffset = terrain.transform.position;
        //terrainOffset.y = 0f;
        //var origin = topViewCamera.transform.position + terrainOffset;
        var direction = Vector3.down;
        var size = terrain.terrainData.size;
        var x = size.x;
        var h = size.y;
        var originOffset = defaultPos + Vector3.up * y;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < h; j++)
            {
                var origin = originOffset + Vector3.right * i + Vector3.forward * j;
                if (Physics.Raycast(origin, direction, out var hit, Mathf.Infinity))
                {
                    if (1 << hit.collider.gameObject.layer != Layers.groundLayer) continue;
                    Debug.Log("Œ©‚Â‚¯‚½‚æ");
                    var point = hit.point;
                    playerOffset = point - playerTra.position;
                    playerOffset.y = 0f;
                    var targetPos = point;
                    targetPos.y = terrain.SampleHeight(targetPos);
                    playerTra.position = targetPos;
                    var topViewTargetPos = targetPos + Vector3.up * y;
                    topViewCameraMover.transform.position = topViewTargetPos;
                    topViewCameraMover.isSetuped = true;
                    goto Found;
                }
            }
        }

        Found:;
        //if (Physics.Raycast(origin, direction, out var hit, Mathf.Infinity))
        //{
        //    var point = hit.point;
        //    playerOffset = point - playerTra.position;
        //    playerOffset.y = 0f;
        //    var targetPos = point;
        //    targetPos.y = terrain.SampleHeight(targetPos);
        //    playerTra.position = targetPos;
        //}
        //else throw new System.Exception();
    }
    //void SetTerrainPos(Vector3 offset)
    //{
    //    var currentPos = terrain.transform.position;
    //    var targetPos = currentPos + offset;
    //    terrain.transform.position = targetPos;
    //}
}
