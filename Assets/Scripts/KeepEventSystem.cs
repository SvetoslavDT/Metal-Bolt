using UnityEngine;

public class KeepEventSystem : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
