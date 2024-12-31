namespace AF
{
    using AF.Health;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIWeaponStatsContainer : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public IconsDatabase iconsDatabase;


        [Header("Components")]
        public VisualTreeAsset weaponStatsContainer;
        public VisualTreeAsset attributeIndicator;

        public void PreviewWeaponDamageDifference(string currentDamageLabel, Damage currentWeaponDamage, Damage nextWeaponDamage, VisualElement root)
        {
            var currentStatsRoot = weaponStatsContainer.CloneTree();
            currentStatsRoot.Q<VisualElement>("AttributeIndicatorContainer").Add(CreateLabel(currentDamageLabel, 10));
            root.Q<VisualElement>("WeaponStatsContainer").Add(currentStatsRoot);
            UpdateWeaponDamageUI(currentStatsRoot, currentWeaponDamage, nextWeaponDamage);
        }

        public Label CreateLabel(string text, int marginBottom)
        {
            Label newLabel = new() { text = text };

            newLabel.style.marginBottom = marginBottom;
            newLabel.AddToClassList("label-text");
            return newLabel;
        }

        void UpdateWeaponDamageUI(VisualElement root, Damage currentWeaponDamage, Damage desiredWeaponDamage)
        {

            bool isPortuguese = Glossary.IsPortuguese();

            // Update damage types
            UpdateDamageUI(root, isPortuguese ? "Ataque Físico" : "Physical Attack", iconsDatabase.physicalAttack, currentWeaponDamage.physical, desiredWeaponDamage.physical);
            UpdateDamageUI(root, isPortuguese ? "Ataque de Fogo" : "Fire Attack", iconsDatabase.fire, currentWeaponDamage.fire, desiredWeaponDamage.fire);
            UpdateDamageUI(root, isPortuguese ? "Ataque de Gelo" : "Frost Attack", iconsDatabase.frost, currentWeaponDamage.frost, desiredWeaponDamage.frost);
            UpdateDamageUI(root, isPortuguese ? "Ataque Mágico" : "Magic Attack", iconsDatabase.magic, currentWeaponDamage.magic, desiredWeaponDamage.magic);
            UpdateDamageUI(root, isPortuguese ? "Ataque Trovão" : "Lightning Attack", iconsDatabase.lightning, currentWeaponDamage.lightning, desiredWeaponDamage.lightning);
            UpdateDamageUI(root, isPortuguese ? "Ataque das Trevas" : "Darkness Attack", iconsDatabase.darkness, currentWeaponDamage.darkness, desiredWeaponDamage.darkness);
            UpdateDamageUI(root, isPortuguese ? "Ataque Mágico" : "Magic Attack", iconsDatabase.magic, currentWeaponDamage.magic, desiredWeaponDamage.magic);
            UpdateDamageUI(root, isPortuguese ? "Ataque Aquático" : "Water Attack", iconsDatabase.water, currentWeaponDamage.water, desiredWeaponDamage.water);

            if (currentWeaponDamage.statusEffects != null && currentWeaponDamage.statusEffects.Length > 0)
            {
                foreach (var statusEffect in currentWeaponDamage.statusEffects)
                {
                    UpdateDamageUI(root, statusEffect.statusEffect.GetName(), statusEffect.statusEffect.icon, statusEffect.amountPerHit, statusEffect.amountPerHit);
                }
            }

        }

        private void UpdateDamageUI(
            VisualElement root,
            string attributeName,
            Sprite icon,
            float currentValue,
            float desiredValue)
        {
            var label = attributeIndicator.CloneTree();
            label.Q<Label>("StatName").text = attributeName + ": ";

            Label currentValueLabel = label.Q<Label>("CurrentValue");
            currentValueLabel.text = desiredValue.ToString();

            currentValueLabel.style.marginLeft = 10;

            if (desiredValue > currentValue)
            {
                currentValueLabel.style.color = Color.green;
            }
            else if (desiredValue < currentValue)
            {
                currentValueLabel.style.color = Color.red;
            }

            label.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(icon);

            root.Q<VisualElement>("AttributeIndicatorContainer").Add(label);
        }

    }
}
