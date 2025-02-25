namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(ButtonEnhancer))]
    public class InventorySlotButton : MonoBehaviour
    {
        Button _button => GetComponent<Button>();
        public Button Button => _button;
        ButtonEnhancer buttonEnhancer => GetComponent<ButtonEnhancer>();

        [Header("UI")]
        [SerializeField] Sprite _backgroundIcon;
        public Sprite BackgroundIcon
        {
            set
            {
                this._backgroundIcon = value;
            }
        }
        [SerializeField] Image equippedIndicator;

        // Events
        public UnityAction onSelect, onDeselect, onPointerEnter, onPointerExit;

        public void ShowEquippedIcon() => equippedIndicator.gameObject.SetActive(true);
        public void HideEquippedIcon() => equippedIndicator.gameObject.SetActive(false);

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
    }
}
