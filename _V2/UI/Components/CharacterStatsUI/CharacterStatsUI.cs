namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CharacterStatsUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] CharacterStats characterStats;

        [Header("UI Components")]
        [SerializeField] Slider healthSlider;
        [SerializeField] TextMeshProUGUI healthValues;
        [SerializeField] Slider staminaSlider;
        [SerializeField] TextMeshProUGUI staminaValues;
        [SerializeField] Slider manaSlider;
        [SerializeField] TextMeshProUGUI manaValues;

        void Awake()
        {
            if (characterStats != null)
            {
                characterStats.onHealthChange.AddListener(UpdateHealth);
                characterStats.CharacterStamina.onStaminaChange.AddListener(UpdateStamina);
                characterStats.onManaChange.AddListener(UpdateMana);
            }
        }

        void UpdateStat(Slider slider, float currentValue, float maxValue, TextMeshProUGUI indicator)
        {
            float normalizedValue = Mathf.Clamp01(currentValue / maxValue);
            slider.value = normalizedValue;

            indicator.text = $"{(int)currentValue} / {(int)maxValue}";
        }

        void UpdateHealth() => UpdateStat(healthSlider, characterStats.Health, characterStats.MaxHealth, healthValues);

        void UpdateStamina() => UpdateStat(staminaSlider, characterStats.CharacterStamina.Stamina, characterStats.CharacterStamina.MaxStamina, staminaValues);

        void UpdateMana() => UpdateStat(manaSlider, characterStats.Mana, characterStats.MaxMana, manaValues);

    }
}
