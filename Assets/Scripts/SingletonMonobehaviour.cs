using UnityEngine;

public class SingletonMonobehaviour<TMono> : MonoBehaviour where TMono : MonoBehaviour
{
    public  static TMono Instance { get; private set;}

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as TMono;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
}
