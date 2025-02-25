namespace AFV2
{
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponHitbox : MonoBehaviour
    {
        new Collider collider => GetComponent<Collider>();
        AudioSource audiSource => GetComponent<AudioSource>();

        [Header("Trail Renderer")]
        public TrailRenderer trailRenderer;

        [Header("Sounds")]
        public AudioClip[] swings;

        void Awake()
        {
            if (trailRenderer != null)
            {
                trailRenderer.emitting = false;
            }

            audiSource.playOnAwake = false;
            AudioUtils.Setup3DAudioSource(audiSource);
        }

        public void EnableHitbox()
        {
            if (swings.Length > 0)
            {
                audiSource.PlayOneShot(swings[Random.Range(0, swings.Length)]);
            }

            collider.enabled = true;

            if (trailRenderer != null)
            {
                trailRenderer.emitting = true;
            }
        }

        public void DisableHitbox()
        {
            collider.enabled = false;

            if (trailRenderer != null)
            {
                trailRenderer.emitting = false;
            }
        }
    }
}
