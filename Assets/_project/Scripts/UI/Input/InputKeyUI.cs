namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class InputKeyUI : MonoBehaviour
    {
        PlayerInput playerInput;

        [Header("Keyboard")]
        [SerializeField] GameObject keyboardContainer;
        [SerializeField] TextMeshProUGUI keyboardLabel;

        [Header("PS4")]
        [SerializeField] GameObject ps4Container;
        [SerializeField] Image ps4Icon;

        [Header("Xbox")]
        [SerializeField] GameObject xboxContainer;
        [SerializeField] Image xboxIcon;

        [Header("Desired Input")]
        public InputAction desiredInputAction;

        void OnEnable()
        {
            if (playerInput == null)
                playerInput = FindAnyObjectByType<PlayerInput>();

            DisableAll();

            HandleIcon();
        }

        void HandleIcon()
        {
            if (playerInput.currentControlScheme == "KeyboardMouse")
            {
                EnableKeyboard();
            }
            else if (playerInput.currentControlScheme == "")
            {
                EnablePS4();
            }
            else if (playerInput.currentControlScheme == "")
            {
                EnableXbox();
            }
        }

        void DisableAll()
        {
            keyboardContainer.SetActive(false);
            xboxContainer.SetActive(false);
            ps4Container.SetActive(false);
        }

        void EnableKeyboard()
        {
            keyboardLabel.text = desiredInputAction.GetCurrentKeyBinding(playerInput);

            keyboardContainer.SetActive(true);
        }

        void EnablePS4()
        {
            ps4Icon.sprite = desiredInputAction.ps4Icon;

            ps4Container.SetActive(true);
        }

        void EnableXbox()
        {
            xboxIcon.sprite = desiredInputAction.xboxIcon;

            xboxContainer.SetActive(true);
        }
    }
}