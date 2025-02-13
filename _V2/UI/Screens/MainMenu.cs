namespace AFV2
{
    using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject[] childScreens;

        bool isActive = false;
        public bool IsActive => isActive;

        public void Show()
        {
            UIUtils.EnableCursor();

            isActive = true;

            if (childScreens.Length > 0)
                UIUtils.FadeIn(childScreens[0]);
        }

        public void Hide()
        {
            UIUtils.DisableCursor();

            isActive = false;

            foreach (var child in childScreens)
            {
                UIUtils.FadeOut(child);
            }
        }
    }
}
