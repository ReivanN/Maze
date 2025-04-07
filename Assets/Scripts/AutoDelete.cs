using UnityEngine;

public class AutoDelete : MonoBehaviour
{
    public float timeToDestroy = 5f;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
