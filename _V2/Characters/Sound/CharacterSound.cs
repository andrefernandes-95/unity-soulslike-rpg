namespace AFV2
{
    using AF;
    using UnityEngine;

    public class CharacterSound : MonoBehaviour
    {
        [SerializeField] private AudioSource[] audioSources;
        [SerializeField] private int maxAudioSources = 3;

        void Awake()
        {
            if (audioSources == null || audioSources.Length == 0)
            {
                audioSources = new AudioSource[maxAudioSources];
                for (int i = 0; i < maxAudioSources; i++)
                {
                    audioSources[i] = gameObject.AddComponent<AudioSource>();
                    audioSources[i].playOnAwake = false;
                    AudioUtils.Setup3DAudioSource(audioSources[i]);
                }
            }
        }

        public void PlaySound(AudioClip audioClip)
        {
            AudioSource selectedSource = null;

            // Find the first available (not playing) audio source
            foreach (var source in audioSources)
            {
                if (!source.isPlaying)
                {
                    selectedSource = source;
                    break;
                }
            }

            // If all are playing, fallback to the first one
            if (selectedSource == null)
            {
                selectedSource = audioSources[0];
            }

            selectedSource.PlayOneShot(audioClip);
        }
    }
}
