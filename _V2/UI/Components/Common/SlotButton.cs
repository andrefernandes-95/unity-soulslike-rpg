namespace AFV2
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class SlotButton : MonoBehaviour
    {
        [Header("Slot Settings")]
        public EquipmentSlotType equipmentSlotType;

        [Header("Components")]
        protected Button button => GetComponent<Button>();
        public Button Button => button;
        public Image itemIcon;

        void Start()
        {
            RegisterEvents();
        }

        protected void RegisterEvents()
        {
            if (!button.TryGetComponent(out EventTrigger eventTrigger))
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            AddTrigger(eventTrigger, EventTriggerType.Select, OnSelect);
            AddTrigger(eventTrigger, EventTriggerType.Deselect, OnDeselect);
        }

        void AddTrigger(EventTrigger eventTrigger, EventTriggerType eventId, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry Entry = new() { eventID = eventId };
            Entry.callback.AddListener(callback);
            eventTrigger.triggers.Add(Entry);
        }

        protected virtual void OnSelect(BaseEventData eventData)
        {
        }
        protected virtual void OnDeselect(BaseEventData eventData)
        {
        }
    }
}
