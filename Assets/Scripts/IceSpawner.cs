using UnityEngine;

public class IceSpawner : MonoBehaviour
{
    [Tooltip("������ �������� ��� ���������� ������")]
    public GameObject[] objectsToSpawn;

    [Tooltip("���������� �������� ��� ������")]
    public int spawnCount = 5;

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (objectsToSpawn == null || objectsToSpawn.Length == 0)
        {
            Debug.LogWarning("������ �������� ��� ������ ����.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-0.5f, 0.5f)
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }
}
