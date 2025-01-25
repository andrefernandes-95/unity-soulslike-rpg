namespace AF
{
    using AF.StatusEffects;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CharacterStatusEffectIndicator : MonoBehaviour
    {
        public Slider slider;

        public Image fill;

        public TextMeshProUGUI label;

        public void UpdateUI(AppliedStatusEffect statusEffect, float currentMaximumResistanceToStatusEffect)
        {
            label.text = statusEffect.hasReachedTotalAmount ? statusEffect.statusEffect.GetAppliedName() : statusEffect.statusEffect.GetName();
            slider.value = statusEffect.currentAmount;
            slider.maxValue = currentMaximumResistanceToStatusEffect;

            fill.color = statusEffect.statusEffect.barColor;
        }
    }

}
