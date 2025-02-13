namespace AFV2
{
    using DG.Tweening;
    using UnityEngine;

    public static class UIUtils
    {
        public static void FadeIn(GameObject gameObject, float fadeDuration = 0.5f)
        {
            if (!gameObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1, fadeDuration);
        }


        public static void FadeOut(GameObject gameObject, float fadeDuration = 0.5f)
        {
            if (!gameObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 1;
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.DOFade(0, fadeDuration);
        }

        public static void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}