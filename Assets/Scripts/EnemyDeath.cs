using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void OnEnable() 
    {
        enemy.EnemyDeath += Death;
    }

    private void OnDisable()
    {
        enemy.EnemyDeath -= Death;
    }
    public void Death() 
    {
        StartCoroutine(Died());
    }
    IEnumerator Died()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
}
