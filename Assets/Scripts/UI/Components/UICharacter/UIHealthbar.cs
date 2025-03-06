namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIHealthbar : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] CharacterHealth characterHealth;

        [Header("UI Components")]
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI value;

        void Awake()
        {
            if (characterHealth != null)
            {
                characterHealth.onHealthChange.AddListener(UpdateStat);
            }
        }

        void UpdateStat() => UIUtils.UpdateStat(slider, characterHealth.Health, characterHealth.MaxHealth, value);
    }
}
