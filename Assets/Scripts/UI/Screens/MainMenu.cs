namespace AFV2
{
    using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] MainHUD playerHud;
        [SerializeField] GameObject navbar;
        [SerializeField] MainMenuScreen[] menuScreens;

        void Awake()
        {
            Hide();
        }

        public void SetScreen(MainMenuScreen mainMenuScreen)
        {
            DisableAllScreens();
            mainMenuScreen.Show();
        }

        public void Show()
        {
            UIUtils.EnableCursor();

            if (menuScreens.Length > 0)
            {
                SetScreen(menuScreens[0]);
            }

            DisableOtherUIs();

            navbar.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            UIUtils.DisableCursor();

            DisableAllScreens();
            EnableOtherUIs();

            navbar.SetActive(false);
            gameObject.SetActive(false);
        }

        void DisableAllScreens()
        {
            foreach (var child in menuScreens)
            {
                child.Hide();
            }
        }

        void DisableOtherUIs()
        {
            playerHud.Hide();
        }

        void EnableOtherUIs()
        {
            playerHud.Show();
        }

        public bool IsActive() => gameObject.activeSelf;
    }
}
