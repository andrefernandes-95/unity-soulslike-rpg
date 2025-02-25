namespace AFV2
{
    using UnityEngine;

    public class CharacterEquipment : MonoBehaviour
    {
        public CharacterWeapons characterWeapons;

        [SerializeField] private Arrow[] arrows = new Arrow[2];
        public Arrow[] Arrows => arrows;
        [SerializeField] private Skill[] skills = new Skill[6];
        public Skill[] Skills => skills;
        [SerializeField] private Consumable[] consumables = new Consumable[6];
        public Consumable[] Consumables => consumables;
        [SerializeField] private Headgear headgear;
        public Headgear Headgear => headgear;
        [SerializeField] private Armor armor;
        public Armor Armor => armor;
        [SerializeField] private Boot boots;
        public Boot Boots => boots;
        [SerializeField] private Accessory[] accessories = new Accessory[4];
        public Accessory[] Accessories => accessories;

        public void EquipSkill(Skill skill, int slot)
        {
            bool shouldUnequip = skills[slot] == skill;
            UnequipSkill(slot);
            if (shouldUnequip) return;

            skills[slot] = skill;
        }

        public void UnequipSkill(int slot = 0)
        {
            skills[slot] = null;
        }

        public void EquipArrow(Arrow arrow, int slot)
        {
            bool shouldUnequip = arrows[slot] == arrow;
            UnequipArrow(slot);
            if (shouldUnequip) return;

            arrows[slot] = arrow;
        }

        public void UnequipArrow(int slot)
        {
            arrows[slot] = null;
        }

        public void EquipConsumable(Consumable consumable, int slot)
        {
            bool shouldUnequip = consumables[slot] == consumable;
            UnequipConsumable(slot);
            if (shouldUnequip) return;

            consumables[slot] = consumable;
        }

        public void UnequipConsumable(int slot)
        {
            consumables[slot] = null;
        }

        public void EquipHeadgear(Headgear newHeadgear)
        {
            bool shouldUnequip = headgear == newHeadgear;
            UnequipHeadgear();
            if (shouldUnequip) return;

            headgear = newHeadgear;
            headgear.Equip();
        }

        public void UnequipHeadgear()
        {
            if (headgear != null)
                headgear.Unequip();

            headgear = null;
        }

        public void EquipArmor(Armor newArmor)
        {
            bool shouldUnequip = armor == newArmor;
            UnequipArmor();
            if (shouldUnequip) return;

            armor = newArmor;
            armor.Equip();
        }

        public void UnequipArmor()
        {
            if (armor != null)
                armor.Unequip();

            armor = null;
        }

        public void EquipBoots(Boot newBoots)
        {
            bool shouldUnequip = boots == newBoots;
            UnequipBoots();
            if (shouldUnequip) return;

            boots = newBoots;
            boots.Equip();
        }

        public void UnequipBoots()
        {
            if (boots != null)
                boots.Unequip();

            boots = null;
        }

        public void EquipAccessory(Accessory accessory, int slot)
        {
            bool shouldUnequip = accessories[slot] == accessory;
            UnequipAccessory(slot);
            if (shouldUnequip) return;

            accessories[slot] = accessory;
        }

        public void UnequipAccessory(int slot)
        {
            accessories[slot] = null;
        }

    }
}
