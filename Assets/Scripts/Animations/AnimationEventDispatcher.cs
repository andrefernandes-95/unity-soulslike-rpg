namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class AnimationEventDispatcher : MonoBehaviour
    {
        [SerializeField] AnimatorManager animatorManager;

        public Dictionary<string, UnityEvent> commonActions = new();
        public Dictionary<string, UnityEvent> leftHandActions = new();
        public Dictionary<string, UnityEvent> rightHandActions = new();

        public UnityEngine.AnimationEvent RegisterEvent(string animationName, AFV2.AnimationEvent animationEvent, Dictionary<string, UnityEvent> targetDictionary, float animationLength)
        {
            string eventName = $"{animationName}_{animationEvent.triggerTime}";

            if (!targetDictionary.ContainsKey(eventName))
            {
                targetDictionary[eventName] = animationEvent.OnEvent;
            }

            // Create and assign AnimationEvent
            var createdAnimationEvent = new UnityEngine.AnimationEvent
            {
                functionName = "TriggerEvent",
                stringParameter = eventName,
                time = animationEvent.triggerTime * animationLength,
            };

            return createdAnimationEvent;
        }

        // Method that Unity AnimationEvent will call
        public void TriggerEvent(string eventName)
        {
            TryTriggerEvent(commonActions, eventName);
            TryTriggerEvent(leftHandActions, eventName);
            TryTriggerEvent(rightHandActions, eventName);
        }

        void TryTriggerEvent(Dictionary<string, UnityEvent> targetDictionary, string eventName)
        {
            if (targetDictionary.TryGetValue(eventName, out UnityEvent animationEvent))
            {
                animationEvent?.Invoke();
            }
        }
    }
}
