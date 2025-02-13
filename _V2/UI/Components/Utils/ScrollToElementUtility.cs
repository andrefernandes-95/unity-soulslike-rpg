namespace AFV2
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class ScrollToElementUtility : MonoBehaviour
    {
        ScrollRect scrollRect => GetComponent<ScrollRect>();


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
            RectTransform viewportRect = scrollRect.viewport;

            // Convert target position to local position in the ScrollRect
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRect, target.position, null, out localPoint);

            // Calculate the normalized scroll position (0 = top, 1 = bottom)
            float scrollPercentage = Mathf.Clamp01(1 - (localPoint.y / contentRect.rect.height));

            // Apply the scroll position
            scrollRect.verticalNormalizedPosition = scrollPercentage;
        }
    }
}
