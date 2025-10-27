using UnityEngine;
using Game.Player;
public class PosaitionSetUper : MonoBehaviour
{
    [SerializeField] Camera topViewCamera;
    Terrain terrain;
 
    public void Initialize(Transform playerTra)
    {
        terrain = Terrain.activeTerrain;
        SetPlayerPos(out var offset,playerTra);
        SetTerrainPos(offset);
    }
    void SetPlayerPos(out Vector3 playerOffset,Transform playerTra)
    {
        playerOffset = default;
        var origin = topViewCamera.transform.position;
        var direction = Vector3.down;
        if (Physics.Raycast(origin, direction, out var hit, Mathf.Infinity, Layers.groundLayer))
        {
            var point = hit.point;
            playerOffset = point - playerTra.position;
            playerOffset.y = 0f;
            var targetPos = point;
            targetPos.y = terrain.SampleHeight(targetPos);
            playerTra.position = targetPos;
        }
        else throw new System.Exception();
    }

    void SetTerrainPos(Vector3 offset)
    {
        var currentPos = terrain.transform.position;
        var targetPos = currentPos + offset;
        terrain.transform.position = targetPos;
    }

}
