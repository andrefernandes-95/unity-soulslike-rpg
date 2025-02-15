namespace AFV2
{
    using AF;
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class AudioSource3D : MonoBehaviour
    {
        AudioSource audioSource => GetComponent<AudioSource>();

        private void Awake()
        {

            audioSource.playOnAwake = false;
            AudioUtils.Setup3DAudioSource(audioSource);
        }
    }
}
