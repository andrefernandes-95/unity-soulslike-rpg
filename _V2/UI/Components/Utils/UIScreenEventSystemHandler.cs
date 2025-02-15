namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UIScreenEventSystemHandler : MonoBehaviour
    {
        List<Selectable> selectables = new();

        Selectable lastSelectable;

        void Start()
        {
            AssignSelectables();
            AttachToInputListener();
        }

        void AttachToInputListener()
        {
            InputListener inputListener = FindAnyObjectByType<InputListener>(FindObjectsInactive.Include);
            if (inputListener == null) return;

            inputListener.onNavigate.AddListener(OnNavigate);
        }

        void OnEnable()
        {
            Invoke(nameof(SelectFirstGameObject), 0f);
        }

        void AssignSelectables()
        {
            this.selectables = transform.GetComponentsInChildren<Selectable>().ToList();

            foreach (var selectable in selectables)
            {
                if (!selectable.TryGetComponent(out EventTrigger eventTrigger))
                {
                    eventTrigger = selectable.gameObject.AddComponent<EventTrigger>();
                }

                AddTrigger(eventTrigger, EventTriggerType.Select, OnSelect);
                AddTrigger(eventTrigger, EventTriggerType.Deselect, OnDeselect);
                AddTrigger(eventTrigger, EventTriggerType.PointerEnter, OnPointerEnter);
                AddTrigger(eventTrigger, EventTriggerType.PointerExit, OnPointerExit);
            }
        }

        void AddTrigger(EventTrigger eventTrigger, EventTriggerType eventId, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry Entry = new() { eventID = eventId };
            Entry.callback.AddListener(callback);
            eventTrigger.triggers.Add(Entry);
        }

        void OnSelect(BaseEventData eventData)
        {
            lastSelectable = eventData.selectedObject.gameObject.GetComponent<Selectable>();
        }
        void OnDeselect(BaseEventData eventData)
        {

        }
        void OnPointerEnter(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;

            if (pointerEventData == null) return;

            Selectable selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (selectable == null)
                selectable = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();

            pointerEventData.selectedObject = selectable.gameObject;
        }

        void OnPointerExit(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;

            if (pointerEventData == null) return;

            pointerEventData.selectedObject = null;
        }

        void SelectFirstGameObject()
        {
            if (selectables.Count > 0)
            {
                Selectable match = selectables.FirstOrDefault(selectable => selectable != null);
                if (match != null) match.Select();
            }
        }

        void OnNavigate()
        {
            if (EventSystem.current.currentSelectedGameObject == null && lastSelectable != null)
                EventSystem.current.SetSelectedGameObject(lastSelectable.gameObject);
        }

    }
}