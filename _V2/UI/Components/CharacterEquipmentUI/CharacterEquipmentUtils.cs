namespace AFV2
{
    using UnityEngine;

    public static class CharacterEquipmentUtils
    {

        // Add your logic here to get the equipped item slot
        public static Item GetEquippedItemSlot(CharacterEquipment characterEquipment, EquipmentSlotType equipmentSlotType, int slotIndex)
        {
            if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
                return characterEquipment.characterWeapons.RightWeapons[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
                return characterEquipment.characterWeapons.LeftWeapons[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.ARROW)
                return characterEquipment.Arrows[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.SKILL)
                return characterEquipment.Skills[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.ACCESSORY)
                return characterEquipment.Accessories[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.CONSUMABLE)
                return characterEquipment.Consumables[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.HEADGEAR)
                return characterEquipment.Headgear;
            if (equipmentSlotType == EquipmentSlotType.ARMOR)
                return characterEquipment.Armor;
            if (equipmentSlotType == EquipmentSlotType.BOOTS)
                return characterEquipment.Boots;

            return null;

        }
    }
}