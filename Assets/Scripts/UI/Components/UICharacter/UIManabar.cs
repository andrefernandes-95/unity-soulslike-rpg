namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManabar : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] CharacterMana characterMana;

        [Header("UI Components")]
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI value;

        void Awake()
        {
            if (characterMana != null)
            {
                characterMana.onManaChange.AddListener(UpdateStat);
            }
        }

        void UpdateStat() => UIUtils.UpdateStat(slider, characterMana.Mana, characterMana.MaxMana, value);
    }
}
