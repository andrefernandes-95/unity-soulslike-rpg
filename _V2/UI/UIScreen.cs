namespace AFV2
{
    using UnityEngine;

    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private CanvasGroup CanvasGroup => canvasGroup ??= GetComponent<CanvasGroup>();

        [Header("Settings")]
        [SerializeField] private bool hideOnAwake = true;

        private void Awake()
        {
            if (hideOnAwake) ForceHide();
        }

        public virtual void Show()
        {
            UIUtils.FadeIn(this.gameObject);
        }

        public virtual void Hide()
        {
            UIUtils.FadeOut(this.gameObject);
        }

        public bool IsActive() => CanvasGroup.alpha > 0;

        private void ForceHide()
        {
            CanvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}
