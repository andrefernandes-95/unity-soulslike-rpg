namespace AFV2
{
    using UnityEngine;

    public class WeaponSlotButton : SlotButton
    {

        [Header("Slot Settings")]
        int slotIndex;
        bool isRightHand;

        public void Initialize(int slotIndex, bool isRightHand)
        {
            this.slotIndex = slotIndex;
            this.isRightHand = isRightHand;
        }

        void Start()
        {
            AssignCallbacks();
        }

        void OnEnable()
        {
            PopulateButton();
        }

        protected override void AssignCallbacks()
        {
            button.onClick.AddListener(OnClickSlot);
        }

        protected override void OnClickSlot()
        {
            equipmentSlotRow.CharacterEquipmentScreen.ShowItemList();
        }

        protected override void PopulateButton()
        {
            CharacterWeapons characterWeapons = GetCharacterWeapons();

            if (isRightHand)
            {
                DrawWeaponSlot(characterWeapons.RightWeapons[slotIndex]);
                return;
            }

            DrawWeaponSlot(characterWeapons.LeftWeapons[slotIndex]);
        }

        void DrawWeaponSlot(Weapon weapon)
        {
            buttonLabel.text = weapon?.DisplayName;
        }

        CharacterWeapons GetCharacterWeapons()
        {
            return equipmentSlotRow.CharacterEquipmentScreen.CharacterApi.characterEquipment.characterWeapons;
        }

        bool IsUnarmed(Weapon slotWeapon) =>
            slotWeapon == equipmentSlotRow.CharacterEquipmentScreen.CharacterApi.characterEquipment.characterWeapons.FallbackWeapon;
    }
}
