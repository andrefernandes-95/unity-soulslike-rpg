namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIStaminabar : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] CharacterStamina characterStamina;

        [Header("UI Components")]
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI value;

        void Awake()
        {
            if (characterStamina != null)
            {
                characterStamina.onStaminaChange.AddListener(UpdateStat);
            }
        }

        void UpdateStat() => UIUtils.UpdateStat(slider, characterStamina.Stamina, characterStamina.MaxStamina, value);
    }
}
