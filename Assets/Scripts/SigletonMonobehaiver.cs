using UnityEngine;

public class SigletonMonobehaiver<TMono> : MonoBehaviour where TMono : MonoBehaviour
{
    public TMono Instance { get; private set;}

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
