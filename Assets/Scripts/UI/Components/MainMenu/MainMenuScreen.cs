using UnityEngine;

namespace AFV2
{
    public class MainMenuScreen : MonoBehaviour
    {
        public NavbarButton navbarButton;

        public void Show()
        {
            gameObject.SetActive(true);
            navbarButton.Activate();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            navbarButton.Deactivate(); ;
        }

    }
}
