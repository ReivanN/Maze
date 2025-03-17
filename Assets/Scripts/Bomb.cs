using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] MeshRenderer renderers;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartBombEffect();
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void StartBombEffect()
    {
        GameObject particleEffect = Instantiate(particles, transform.position, Quaternion.identity);

        ParticleSystem particleSystem = particleEffect.GetComponent<ParticleSystem>();
        particleSystem.Play();
        particleSystem.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        StartCoroutine(WaitForParticlesToFinish(particleSystem));

        renderers.enabled = false;
    }

    IEnumerator WaitForParticlesToFinish(ParticleSystem particleSystem)
    {
        yield return new WaitUntil(() => !particleSystem.isPlaying);
        SceneManager.LoadScene("MazeScene");
    }
}
