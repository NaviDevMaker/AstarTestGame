using UnityEngine;

public static class Layers
{
    public static int wallLayer => LayerMask.GetMask("Wall");
    public static int groundLayer => LayerMask.GetMask("Ground");

    public static int enemyLayer => LayerMask.GetMask("Enemy");
    public static int itemLayer => LayerMask.GetMask("Item");
}
