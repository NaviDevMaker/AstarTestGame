using UnityEngine;
using Game.Player;
using Game.Stage;
public class PosaitionSetUper : MonoBehaviour
{
    [SerializeField] CameraMover topViewCameraMover;
    Terrain terrain;
    public void Initialize(Transform playerTra)
    {
        terrain = Terrain.activeTerrain;
        SetPlayerPos(playerTra);
        //SetTerrainPos(offset);
    }
    void SetPlayerPos(Transform playerTra)
    {
        var instance = StageGenerator.Instance;
        var defaultPos = instance.defaultPosition;//terrain.transform.position;
        var y = topViewCameraMover.transform.position.y;
        //var topViewTargetPos = defaultPos;
        //topViewTargetPos.y += y;
        //topViewCamera.transform.position = topViewTargetPos;
        //var terrainOffset = terrain.transform.position;
        //terrainOffset.y = 0f;
        //var origin = topViewCamera.transform.position + terrainOffset;
        var direction = Vector3.down;
        //var size = terrain.terrainData.size;
        var x = instance.map.GetLength(0);
        var h = instance.map.GetLength(1);
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
                    var node = new Vector2Int(i,j);
                    var targetPos = StageMethods.GetTargetNodePos(new MapPositionInfo(node.x,node.y));
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
