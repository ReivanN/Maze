using System.Collections;
using UnityEngine;

public class EnemyDeathMelee : MonoBehaviour
{
    [SerializeField] private EnemyMelee enemy;
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
