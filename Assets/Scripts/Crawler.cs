using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public class Crawler : MonoBehaviour
{
    /*public float patrolWaitTime = 2f;  // Время ожидания на каждой точке
    public float patrolRadius = 10f;  // Радиус поиска точки для патруля
    public NavMeshSurface navMeshSurface;  // Ссылка на NavMeshSurface
    private NavMeshAgent agent;  // Ссылка на NavMeshAgent

    void Start()
    {
        agent = FindAnyObjectByType<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден! Добавьте его к объекту.");
            return;
        }

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface не задан! Укажите его в инспекторе.");
            return;
        }

        StartCoroutine(Patrol());  // Запускаем патрулирование
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            // Находим случайную точку в пределах заданного радиуса
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;  // Смещаем точку относительно текущей позиции

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);  // Устанавливаем точку назначения на NavMesh
                Debug.Log("Новая точка назначения: " + hit.position);
            }

            // Ожидаем, пока слизень достигнет точки назначения
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Ждем некоторое время на текущей точке
            yield return new WaitForSeconds(patrolWaitTime);
        }
    }*/
}
