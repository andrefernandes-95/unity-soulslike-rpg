namespace AFV2
{
    using DG.Tweening;
    using UnityEngine;

    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        private CharacterApi characterApi;
        public CharacterApi CharacterApi => characterApi;

        private CanvasGroup canvasGroup;
        private CanvasGroup CanvasGroup => canvasGroup ??= GetComponent<CanvasGroup>();

        [Header("Settings")]
        [SerializeField] private float fadeSpeed = 0.5f;
        [SerializeField] private bool hideOnAwake = true;

        private void Awake()
        {
            if (hideOnAwake) ForceHide();
        }

        public virtual void Show(CharacterApi characterApi)
        {
            this.characterApi = characterApi;

            gameObject.SetActive(true);
            CanvasGroup.alpha = 0;
            CanvasGroup.DOFade(1, fadeSpeed);
        }

        public virtual void Hide()
        {
            CanvasGroup.DOFade(0, fadeSpeed).OnComplete(() => gameObject.SetActive(false));
        }

        public bool IsActive() => CanvasGroup.alpha > 0;

        private void ForceHide()
        {
            CanvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}
