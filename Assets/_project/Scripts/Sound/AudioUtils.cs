namespace AFV2
{
    using UnityEngine;

    public static class AudioUtils
    {
        public static void Setup3DAudioSource(AudioSource audioSource)
        {
            audioSource.spatialBlend = 1f;
            audioSource.dopplerLevel = 0;
            audioSource.spread = 180;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 5;
            audioSource.maxDistance = 35;
        }
    }
}
