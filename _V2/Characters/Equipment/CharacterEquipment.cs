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
        [SerializeField] private Accessory[] accessories = new Accessory[2];
        public Accessory[] Accessories => accessories;

        public void EquipSkill(Skill skill, int slot)
        {
            UnequipSkill(slot);

            skills[slot] = skill;
        }

        public void UnequipSkill(int slot = 0)
        {
            skills[slot] = null;
        }

        public void EquipArrow(Arrow arrow, int slot)
        {
            UnequipArrow(slot);

            arrows[slot] = arrow;
        }

        public void UnequipArrow(int slot)
        {
            arrows[slot] = null;
        }

        public void EquipConsumable(Consumable consumable, int slot)
        {
            UnequipConsumable(slot);

            consumables[slot] = consumable;
        }

        public void UnequipConsumable(int slot)
        {
            consumables[slot] = null;
        }

        public void EquipHeadgear(Headgear newHeadgear)
        {
            UnequipHeadgear();

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
            UnequipArmor();

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
            UnequipBoots();

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
            UnequipAccessory(slot);

            accessories[slot] = accessory;
        }

        public void UnequipAccessory(int slot)
        {
            accessories[slot] = null;
        }

    }
}
