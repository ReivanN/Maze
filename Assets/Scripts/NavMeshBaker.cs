using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
                instance = FindAnyObjectByType<NavMeshBaker>();
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
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindNavMeshSurface();
        StartCoroutine(BakeNavMeshCoroutine());
    }

    private void FindNavMeshSurface()
    {
        navMeshSurface = FindAnyObjectByType<NavMeshSurface>();

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface �� ������ � �����! ���������, ��� �� ���� � Hierarchy.");
        }
    }

    public void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("���������� ������ NavMesh! NavMeshSurface �� ������.");
        }
    }

    public IEnumerator BakeNavMeshCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        BakeNavMesh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            BakeNavMesh();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
