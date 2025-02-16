namespace AFV2
{
    using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] PlayerHUD playerHud;

        [SerializeField] GameObject[] childScreens;

        bool isActive = false;
        public bool IsActive => isActive;

        public void Show()
        {
            UIUtils.EnableCursor();

            isActive = true;

            this.gameObject.SetActive(true);

            if (childScreens.Length > 0)
                UIUtils.FadeIn(childScreens[0]);

            DisableOtherUIs();
        }

        public void Hide()
        {
            UIUtils.DisableCursor();

            isActive = false;

            foreach (var child in childScreens)
            {
                UIUtils.FadeOut(child);
            }

            this.gameObject.SetActive(false);

            EnableOtherUIs();
        }

        void DisableOtherUIs()
        {
            UIUtils.FadeOut(playerHud.gameObject);
        }

        void EnableOtherUIs()
        {
            UIUtils.FadeIn(playerHud.gameObject);
        }
    }
}
