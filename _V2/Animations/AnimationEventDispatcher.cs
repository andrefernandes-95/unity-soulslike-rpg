namespace AFV2
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Events;

    public class AnimationEventDispatcher : MonoBehaviour
    {
        [SerializeField]
        private SerializedDictionary<string, UnityEvent> eventMap = new();

        // Register a new event dynamically
        public void RegisterEvent(string eventName, UnityEvent unityEvent)
        {
            if (!eventMap.ContainsKey(eventName))
            {
                eventMap[eventName] = unityEvent;
            }
        }

        // Method that Unity AnimationEvent will call
        public void TriggerEvent(string eventName)
        {
            if (eventMap.TryGetValue(eventName, out UnityEvent unityEvent))
            {
                unityEvent?.Invoke();
            }
        }

        public void ClearAll() => eventMap.Clear();
    }

}