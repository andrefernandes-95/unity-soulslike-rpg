namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class NavbarButton : MonoBehaviour
    {
        public MainMenu mainMenu;
        public MainMenuScreen mainMenuScreen;
        public TextMeshProUGUI text;

        Button button => GetComponent<Button>();

        void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            mainMenu.SetScreen(mainMenuScreen);
        }

        public void Activate()
        {
            text.fontStyle = FontStyles.Underline;
        }

        public void Deactivate()
        {
            text.fontStyle = FontStyles.Normal;
        }
    }
}