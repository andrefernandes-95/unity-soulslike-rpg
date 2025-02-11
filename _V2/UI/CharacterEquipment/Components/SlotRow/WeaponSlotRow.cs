namespace AFV2
{
    using AF;
    using UnityEngine;

    public class WeaponSlotRow : EquipmentSlotRow
    {

        [Header("Slot Button")]
        [SerializeField] WeaponSlotButton slotButton;

        [SerializeField] bool isRightHand;

        void OnEnable()
        {
            UpdateLabels();

            ClearSlots();

            InstantiateSlots();
        }

        void InstantiateSlots()
        {
            int slotIndex = 0;
            foreach (Weapon _ in CharacterEquipmentScreen.CharacterApi.characterEquipment.characterWeapons.RightWeapons)
            {
                WeaponSlotButton instance = Instantiate(slotButton, slotsContainer);
                instance.Initialize(slotIndex, isRightHand);
                slotIndex++;
            }
        }

        void UpdateLabels()
        {
            if (isRightHand)
            {
                this.categoryLabel.text = Glossary.IsPortuguese() ? "Mão Direita" : "Right Hand";
                return;
            }

            this.categoryLabel.text = Glossary.IsPortuguese() ? "Mão Esquerda" : "Left Hand";
        }

    }
}