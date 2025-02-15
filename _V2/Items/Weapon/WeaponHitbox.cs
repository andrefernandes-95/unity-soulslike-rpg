namespace AFV2
{
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class WeaponHitbox : MonoBehaviour
    {
        new Collider collider => GetComponent<Collider>();

        [Header("Trail Renderer")]
        public TrailRenderer trailRenderer;

        [Header("Sounds")]
        [SerializeField] CharacterSound characterSound;
        public AudioClip[] swings;

        void Awake()
        {
            if (trailRenderer != null) trailRenderer.emitting = false;
        }

        public void EnableHitbox()
        {
            if (swings.Length > 0)
            {
                characterSound.PlaySound(swings[Random.Range(0, swings.Length)]);
            }

            collider.enabled = true;
            if (trailRenderer != null) trailRenderer.emitting = true;
        }

        public void DisableHitbox()
        {
            collider.enabled = false;
            if (trailRenderer != null) trailRenderer.emitting = false;
        }
    }
}