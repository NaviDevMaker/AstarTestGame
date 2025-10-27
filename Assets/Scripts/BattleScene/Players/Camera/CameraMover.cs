using UnityEngine;

public class CameraMover : MonoBehaviour
{
    Transform playerTra;
    Vector3 previousPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        ChaseTarget();
    }

    public void Initialize(Transform playerTra)
    {
        this.playerTra = playerTra;
        previousPos = playerTra.position;
    }
    void ChaseTarget()
    {
        var offset = playerTra.position - previousPos;
        transform.position += offset;
        previousPos = playerTra.position;
    }
}
