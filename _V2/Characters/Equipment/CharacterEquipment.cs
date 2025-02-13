namespace AFV2
{
    using System.Linq;
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


        [Header("Components")]
        public CharacterInventory characterInventory;

        private void Awake()
        {
            SyncInventoryWithEquipment();

            SyncDefaultEquipment();
        }

        void SyncInventoryWithEquipment()
        {
            foreach (Arrow arrow in arrows.Where(item => item != null))
                characterInventory.AddItem(arrow, 1);
            foreach (Skill skill in skills.Where(item => item != null))
                characterInventory.AddItem(skill, 1);
            foreach (Consumable consumable in consumables.Where(item => item != null))
                characterInventory.AddItem(consumable, 1);

            if (headgear != null) characterInventory.AddItem(headgear, 1);
            if (armor != null) characterInventory.AddItem(armor, 1);
            if (boots != null) characterInventory.AddItem(boots, 1);
        }

        void SyncDefaultEquipment()
        {
            if (headgear != null) headgear.OnEquip.Invoke();
            if (armor != null) armor.OnEquip.Invoke();
            if (boots != null) boots.OnEquip.Invoke();
        }

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
            headgear.OnEquip.Invoke();
        }

        public void UnequipHeadgear()
        {
            if (headgear != null)
                headgear.OnUnequip.Invoke();

            headgear = null;
        }

        public void EquipArmor(Armor newArmor)
        {
            UnequipArmor();

            armor = newArmor;
            armor.OnEquip.Invoke();
        }

        public void UnequipArmor()
        {
            if (armor != null)
                armor.OnUnequip.Invoke();

            armor = null;
        }

        public void EquipBoots(Boot newBoots)
        {
            UnequipBoots();

            boots = newBoots;
            boots.OnEquip.Invoke();
        }

        public void UnequipBoots()
        {
            if (boots != null)
                boots.OnUnequip.Invoke();

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
