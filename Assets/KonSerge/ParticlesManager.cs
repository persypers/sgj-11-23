using System.Collections;
using System.Collections.Generic;
using Fancy;
using UnityEngine;
using System.Linq;

public class ParticlesManager : Fancy.MonoSingleton<ParticlesManager>
{
    public ObjectPool particlesPool;

    void Start()
    {
    }

    void Update()
    {
    }

    public void ShowParticlesOnTarget(GameObject target, int particlesAmount)
    {
        var col = target.GetComponent<Collider>();
        var bounds = col == null ? new Bounds(target.transform.position, new Vector3(0.5f, 0.5f, 0.5f)) : target.GetComponent<Collider>().bounds;
        for (int i = 0; i < particlesAmount; i++)
        {
            var go = particlesPool.Get();
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y) - 0.6f,
                Random.Range(bounds.min.z, bounds.max.z));
            go.transform.position = pos;
            go.transform.localRotation = Quaternion.Euler(
                Random.Range(-15.0f, 15.0f),
                Random.Range(-5.0f, 5.0f),
                Random.Range(0.0f, 360.0f));
            go.SetActive(true);
            go.GetComponentInChildren<ParticleSystem>().Play();
            StartCoroutine(DisableParticles(go));
        }
        // if (burnSounds.Length > 0)
        // {
        //     var audio = burnSounds[Random.Range(0, burnSounds.Length)];
        //     audio.PlayOneShot(audio.clip);
        // }
    }
    IEnumerator DisableParticles(GameObject bo)
    {
        yield return new WaitForSeconds(2.0f);
        bo.SetActive(false);
    }
}