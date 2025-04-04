using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

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
        audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
