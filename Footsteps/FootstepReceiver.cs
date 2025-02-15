namespace AF.Footsteps
{
    using AFV2;
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
        CharacterApi characterApi => GetComponentInParent<CharacterApi>();

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
                        audioSource.pitch = Random.Range(0.98f, 1.02f);
                        audioSource.PlayOneShot(GetClip(clips));
                        nextFootstepTime = Time.time + footstepCooldown; // Set next available time
                    }
                }
            }
        }

        AudioClip GetClip(AudioClip[] clips)
        {
            if (characterApi != null && characterApi.characterEquipment.Boots != null && characterApi.characterEquipment.Boots.footstepOverrides.Count > 0)
            {
                return characterApi.characterEquipment.Boots.footstepOverrides[
                    Random.Range(0, characterApi.characterEquipment.Boots.footstepOverrides.Count)];
            }

            return clips[Random.Range(0, clips.Length)];
        }
    }
}
