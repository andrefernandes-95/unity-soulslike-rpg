namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    public class AnimationEvent
    {
        [Range(0f, 1f)] public float triggerTime;
        public UnityEvent OnEvent;
    }
}
