namespace AFV2
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ActionClip : MonoBehaviour
    {
        public string AnimationHash => this.gameObject.name;

        [Range(0, 1f)][SerializeField] private float previewTime = 0;
        public float PreviewTime => previewTime;

        public AnimationClip animationClip;

        [SerializeField] private AnimationEvent[] events;
        public AnimationEvent[] Events => events;

        private Dictionary<int, float> previousEventTimes = new Dictionary<int, float>(); // Track previous event times


        public async Task Play(AnimatorManager animatorManager, float blend = 0.2f)
        {
            animatorManager.BlendTo(AnimationHash, blend);
            animatorManager.EnableRootMotion();

            await animatorManager.WaitForAnimationToFinish(AnimationHash);

            animatorManager.DisableRootMotion();
        }

        private void OnValidate()
        {
            if (events == null || events.Length == 0)
            {
                previousEventTimes.Clear(); // Clear previous event times if there are no events
                return;
            }

            int modifiedIndex = -1;

            // Check for changes in event times
            for (int i = 0; i < events.Length; i++)
            {
                float eventTime = events[i].triggerTime;

                if (!previousEventTimes.ContainsKey(i))
                {
                    // If the event index is not in the dictionary, add it
                    previousEventTimes[i] = eventTime;
                    modifiedIndex = i;
                }
                else if (previousEventTimes[i] != eventTime)
                {
                    // If the event time has changed, update the dictionary and mark as modified
                    previousEventTimes[i] = eventTime;
                    modifiedIndex = i;
                }
            }

            // If an event was modified, update previewTime
            if (modifiedIndex != -1)
            {
                previewTime = events[modifiedIndex].triggerTime;
                Debug.Log($"Event {modifiedIndex} changed! New trigger time: {previewTime}");
            }
        }
    }
}
