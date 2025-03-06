namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FootstepListener : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] float rayDistanceDownwards = 1f;
        [SerializeField] float audioSourceVolume = 0.15f;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        [Header("Bone References")]
        public Transform leftFootBone;
        public Transform rightFootBone;

        [Header("Database")]
        [SerializeField] List<FootstepClip> footstepClips = new();

        private FootstepReceiver leftFootReceiver;
        private FootstepReceiver rightFootReceiver;

        private void Awake()
        {
            leftFootReceiver = SetupFoot(leftFootBone);
            rightFootReceiver = SetupFoot(rightFootBone);
        }

        FootstepReceiver SetupFoot(Transform footBone)
        {
            if (footBone == null)
                return null;

            FootstepReceiver receiver = footBone.gameObject.AddComponent<FootstepReceiver>();
            AudioSource audioSource = footBone.gameObject.AddComponent<AudioSource>();

            audioSource.volume = audioSourceVolume;
            AudioUtils.Setup3DAudioSource(audioSource);
            audioSource.playOnAwake = false;

            receiver.SetupFootstepReceiver(footstepClips, characterApi.characterGravity.GroundLayers, audioSource, rayDistanceDownwards);

            return receiver;
        }

        public void PlayLeftFootstep() => PlayFootstep(leftFootReceiver);
        public void PlayRightFootstep() => PlayFootstep(rightFootReceiver);

        void PlayFootstep(FootstepReceiver receiver)
        {
            if (receiver == null)
                return;

            receiver.Trigger();
        }
    }
}
