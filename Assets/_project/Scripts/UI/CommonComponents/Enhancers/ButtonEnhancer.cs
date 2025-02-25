namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Outline))]
    public class ButtonEnhancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] Texture2DContainer cursor;
        Soundbank _soundbank;

        Button button => GetComponent<Button>();

        Outline outline => GetComponent<Outline>();

        [Header("Events")]
        public UnityAction<PointerEventData> onPointerEnter;
        public UnityAction<PointerEventData> onPointerExit;
        public UnityAction<PointerEventData> onClick;
        public UnityAction<BaseEventData> onSelect;
        public UnityAction<BaseEventData> onDeselect;

        void Awake()
        {
            AssignListeners();

            HandleOutline(false);
        }

        void AssignListeners()
        {
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    if (TryGetSoundBank(out Soundbank soundbank))
                    {
                        soundbank.ButtonClick();
                    }
                });
            }
        }

        #region Misc Handlers
        void PlayButtonEnter()
        {
            if (TryGetSoundBank(out Soundbank soundbank))
            {
                soundbank.ButtonEnter();
            }
        }

        void PlayButtonExit()
        {
            if (TryGetSoundBank(out Soundbank soundbank))
            {
                soundbank.ButtonExit();
            }
        }

        void HandleCursor(bool showCustomCursor)
        {
            if (showCustomCursor && cursor != null && cursor.texture != null)
            {
                Cursor.SetCursor(cursor.texture, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        void HandleOutline(bool enable)
        {
            if (outline != null)
            {
                outline.enabled = enable;
            }
        }
        #endregion

        #region Pointer Handler Implementations
        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayButtonEnter();
            HandleCursor(true);
            HandleOutline(true);

            onPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlayButtonExit();
            HandleCursor(false);
            HandleOutline(false);

            onPointerExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            HandleOutline(true);
            onSelect?.Invoke(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            HandleOutline(false);
            onDeselect?.Invoke(eventData);
        }
        #endregion

        #region Scene References
        bool TryGetSoundBank(out Soundbank soundbank)
        {
            soundbank = _soundbank;

            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank != null;
        }

        #endregion
    }
}
