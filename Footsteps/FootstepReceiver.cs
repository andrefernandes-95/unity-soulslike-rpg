namespace AF.Footsteps
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;

    public class FootstepReceiver : MonoBehaviour
    {

        SerializedDictionary<string, AudioClip[]> footstepClips = new();

        [Header("Raycast Settings")]
        LayerMask detectionLayer;
        [SerializeField] float rayDistanceDownwards = 1f;
        [SerializeField] float footstepCooldown = 0.25f;
        AudioSource audioSource;

        private float nextFootstepTime = 0f; // Time when the next footstep can be played

        public void SetupFootstepReceiver(
            SerializedDictionary<string, AudioClip[]> footstepClips,
            LayerMask detectionLayer,
            AudioSource audioSource,
            float rayDistanceDownwards
        )
        {
            this.footstepClips = footstepClips;
            this.detectionLayer = detectionLayer;
            this.audioSource = audioSource;
            this.rayDistanceDownwards = rayDistanceDownwards;
        }

        public void Trigger()
        {
            if (Time.time < nextFootstepTime) return; // Ensures proper cooldown timing

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistanceDownwards, detectionLayer))
            {
                if (footstepClips.TryGetValue(hit.transform.gameObject.tag, out AudioClip[] clips))
                {
                    if (clips != null && clips.Length > 0)
                    {
                        audioSource.pitch = Random.Range(0.95f, 1.05f);
                        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
                        nextFootstepTime = Time.time + footstepCooldown; // Set next available time
                    }
                }
            }
        }
    }
}
