using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    private static NavMeshBaker instance;
    public NavMeshSurface navMeshSurface;

    public static NavMeshBaker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NavMeshBaker>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("NavMeshBaker");
                    instance = singletonObject.AddComponent<NavMeshBaker>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            BakeNavMesh();
        }
    }
}
