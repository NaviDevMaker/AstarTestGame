using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    public float elapsedGameTime { get; private set; }

    private void Awake() => Instance = this;
    // Update is called once per frame
    void Update() => elapsedGameTime += Time.deltaTime;
}
