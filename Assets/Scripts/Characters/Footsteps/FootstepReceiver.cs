namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FootstepReceiver : MonoBehaviour
    {
        Dictionary<string, AudioClip[]> footstepClipsDictionary = new();

        [Header("Raycast Settings")]
        LayerMask detectionLayer;
        [SerializeField] float rayDistanceDownwards = 1f;
        [SerializeField] float footstepCooldown = 0.25f;
        AudioSource audioSource;
        CharacterApi characterApi => GetComponentInParent<CharacterApi>();

        private float nextFootstepTime = 0f; // Time when the next footstep can be played

        public void SetupFootstepReceiver(
            List<FootstepClip> footstepClips,
            LayerMask detectionLayer,
            AudioSource audioSource,
            float rayDistanceDownwards
        )
        {
            SetupFootstepClips(footstepClips);

            this.detectionLayer = detectionLayer;
            this.audioSource = audioSource;
            this.rayDistanceDownwards = rayDistanceDownwards;
        }

        void SetupFootstepClips(List<FootstepClip> footstepClips)
        {
            foreach (FootstepClip entry in footstepClips)
            {
                if (!footstepClipsDictionary.ContainsKey(entry.tag))
                {
                    footstepClipsDictionary.Add(entry.tag, entry.clips);
                }
            }
        }

        public void Trigger()
        {
            if (Time.time < nextFootstepTime) return; // Ensures proper cooldown timing

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistanceDownwards, detectionLayer))
            {
                if (footstepClipsDictionary.TryGetValue(hit.transform.gameObject.tag, out AudioClip[] clips))
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
            Boot boot = characterApi.characterEquipment?.boot?.item as Boot;

            if (characterApi != null && boot != null && boot.footstepOverrides.Count > 0)
            {
                return boot.footstepOverrides[Random.Range(0, boot.footstepOverrides.Count)];
            }

            return clips[Random.Range(0, clips.Length)];
        }
    }
}
