namespace AFV2
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class ScrollToElementUtility : MonoBehaviour
    {
        ScrollRect scrollRect => GetComponent<ScrollRect>();
        [SerializeField] float offset = 0f;

        void Awake()
        {
            InputListener cachedInputListener = FindAnyObjectByType<InputListener>(FindObjectsInactive.Include);
            if (cachedInputListener != null)
            {
                cachedInputListener.onNavigate.AddListener(CheckScroll);
            }
        }

        private void CheckScroll()
        {
            // Get the currently selected UI element
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject != null)
            {
                // Check if it's part of the scrollRect's content
                RectTransform selectedTransform = selectedObject.GetComponent<RectTransform>();
                if (selectedTransform != null && selectedTransform.IsChildOf(scrollRect.content))
                {
                    ScrollToSelectable(selectedTransform);
                }
            }
        }
        private void ScrollToSelectable(RectTransform target)
        {
            Canvas.ForceUpdateCanvases(); // Ensure UI updates before scrolling

            RectTransform contentRect = scrollRect.content;

            // Get target position relative to the content
            float targetY = -target.anchoredPosition.y - offset; // Ensure correct Y-axis direction

            // Get the height of the scrollable content
            float contentHeight = contentRect.rect.height - targetY;

            // Convert to normalized scroll position (1 = top, 0 = bottom)
            float contentRatio = targetY / contentHeight;

            float nextVerticalNormalizedPosition = Mathf.Clamp01(1 - contentRatio);

            // Apply the scroll position
            scrollRect.verticalNormalizedPosition = nextVerticalNormalizedPosition;
        }
    }
}
