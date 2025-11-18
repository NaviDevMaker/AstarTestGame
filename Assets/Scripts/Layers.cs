using UnityEngine;

public static class Layers
{
    public static int wallLayer => LayerMask.GetMask("Wall");
    public static int groundLayer => LayerMask.GetMask("Ground");

    public static int enemyLayer => LayerMask.GetMask("Enemy");
    public static int itemLayer => LayerMask.GetMask("Item");
    public static int wallLayer_NameTo => LayerMask.NameToLayer("Wall");
    public static int groundLayer_NameTo => LayerMask.NameToLayer("Ground");
    public static int roadLayer_NameTo => LayerMask.NameToLayer("Road");
}
