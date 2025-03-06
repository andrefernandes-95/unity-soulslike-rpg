namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// Used in scrollable inventory lists to represent an item
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(ButtonEnhancer))]
    public class InventorySlotButton : MonoBehaviour
    {
        Button _button => GetComponent<Button>();
        public Button Button => _button;
        ButtonEnhancer buttonEnhancer => GetComponent<ButtonEnhancer>();

        [Header("UI")]
        [SerializeField] Image backgroundIcon;
        [SerializeField] GameObject equippedIndicator;

        // Events
        public UnityAction onSelect, onDeselect, onPointerEnter, onPointerExit;

        public void ShowEquippedIcon() => equippedIndicator.SetActive(true);
        public void HideEquippedIcon() => equippedIndicator.SetActive(false);

        void Awake()
        {
            AssignEventListeners();
            HideEquippedIcon();
        }

        void AssignEventListeners()
        {
            buttonEnhancer.onSelect += OnSelect;
            buttonEnhancer.onDeselect += OnDeselect;
            buttonEnhancer.onPointerEnter += OnPointerEnter;
            buttonEnhancer.onPointerExit += OnPointerExit;
        }

        void OnSelect(BaseEventData baseEventData)
        {
            onSelect?.Invoke();
        }

        void OnDeselect(BaseEventData baseEventData)
        {
            onDeselect?.Invoke();
        }

        void OnPointerEnter(PointerEventData pointerEventData)
        {
            onPointerEnter?.Invoke();
        }

        void OnPointerExit(PointerEventData pointerEventData)
        {
            onPointerExit?.Invoke();
        }

        public void SetBackgroundIcon(Sprite icon)
        {
            backgroundIcon.sprite = icon;
        }
    }
}
