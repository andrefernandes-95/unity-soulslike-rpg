namespace AFV2
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public static class UIUtils
    {
        public static void FadeIn(GameObject gameObject, float fadeDuration = 0.5f)
        {
            gameObject.SetActive(true);
            return;

            if (!gameObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
        }

        public static void FadeOut(GameObject gameObject, float fadeDuration = 0.5f)
        {
            gameObject.SetActive(false);
            return;

            if (!gameObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 1;
            canvasGroup.gameObject.SetActive(false);
        }

        private static IEnumerator FadeCoroutine(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
            onComplete?.Invoke();
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


        public static void UpdateStat(Slider slider, float currentValue, float maxValue, TextMeshProUGUI indicator)
        {
            float normalizedValue = Mathf.Clamp01(currentValue / maxValue);
            slider.value = normalizedValue;
            indicator.text = $"{(int)currentValue} / {(int)maxValue}";
        }
    }
}
