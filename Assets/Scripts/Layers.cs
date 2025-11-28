using UnityEngine;

public static class Layers
{
    public static int wallLayer => LayerMask.GetMask("Wall");
    public static int groundLayer => LayerMask.GetMask("Ground");
    public static int transluEnemyLayer_NameTo => LayerMask.NameToLayer("TranslusentGhost");
    public static int transpaEnemyLayer_NameTo => LayerMask.NameToLayer("TransparentGhost");
    public static int itemLayer => LayerMask.GetMask("Item");
    public static int wallLayer_NameTo => LayerMask.NameToLayer("Wall");
    public static int groundLayer_NameTo => LayerMask.NameToLayer("Ground");
    public static int roadLayer_NameTo => LayerMask.NameToLayer("Road");

    public static void SetLayerInChildren(this GameObject obj,int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerInChildren(child.gameObject,layer);
        }
    }
}
