using UnityEngine;

public class PathNode
{
    public int x;
    public int z;
    public int cost;          // 実コスト
    public float heuristic;   // 推定コスト
    public float totalCost;   // cost + heuristic
    public PathNode parent;

    public PathNode(int x, int z, int cost, float heuristic, PathNode parent)
    {
        this.x = x;
        this.z = z;
        this.cost = cost;
        this.heuristic = heuristic;
        this.totalCost = cost + heuristic;
        this.parent = parent;
    }
}
