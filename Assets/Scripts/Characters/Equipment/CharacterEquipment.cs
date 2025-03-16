namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterEquipment : MonoBehaviour
    {
        public HeadgearInstance headgear;
        public ArmorInstance armor;
        public BootInstance boot;
        public AccessoryInstance[] accessories = new AccessoryInstance[4];
        public UnityEvent onEquipmentChange = new();

        public void EquipHeadgear(HeadgearInstance headgearInstance)
        {
            bool shouldUnequip = headgear == headgearInstance;
            UnequipHeadgear();
            if (shouldUnequip)
            {
                return;
            }

            headgear = headgearInstance;
        }

        public void UnequipHeadgear()
        {
            headgear = null;
        }

        public void EquipArmor(ArmorInstance armorInstance)
        {
            bool shouldUnequip = armor == armorInstance;
            UnequipArmor();
            if (shouldUnequip)
            {
                return;
            }

            armor = armorInstance;
        }

        public void UnequipArmor()
        {
            armor = null;
        }

        public void EquipBoots(BootInstance bootInstance)
        {
            bool shouldUnequip = boot == bootInstance;
            UnequipBoots();
            if (shouldUnequip)
            {
                return;
            }

            boot = bootInstance;
        }

        public void UnequipBoots()
        {
            boot = null;
        }

        public void EquipAccessory(AccessoryInstance accessoryInstance, int slot)
        {
            bool shouldUnequip = accessories[slot] == accessoryInstance;
            UnequipAccessory(slot);
            if (shouldUnequip)
            {
                return;
            }

            accessories[slot] = accessoryInstance;
        }

        public void UnequipAccessory(int slot)
        {
            accessories[slot] = null;
        }

    }
}
