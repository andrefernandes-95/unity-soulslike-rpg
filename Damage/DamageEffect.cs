namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(AudioSource))]
    public class DamageEffect : MonoBehaviour
    {
        new ParticleSystem particleSystem => GetComponent<ParticleSystem>();
        List<AudioSource> audioSources = new();

        private void Awake()
        {
            audioSources = GetComponents<AudioSource>().ToList();
        }

        public void Play()
        {
            particleSystem.Play();
            foreach (var audioSource in audioSources)
            {
                audioSource.Play();
            }
        }
    }
}
