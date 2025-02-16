namespace AFV2
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Events;

    public class AnimationEventDispatcher : MonoBehaviour
    {
        [SerializeField] AnimatorManager animatorManager;

        [SerializeField] private SerializedDictionary<string, UnityEvent> eventMap = new();

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
            if (eventMap.TryGetValue(eventName, out UnityEvent animationEvent))
            {
                animationEvent?.Invoke();

                Debug.Log($"{eventName} triggered at {Time.time}");
            }
        }

        public void ClearAll()
        {
            eventMap.Clear();
        }
    }
}
