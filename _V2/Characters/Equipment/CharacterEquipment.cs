namespace AFV2
{
    using System.Linq;
    using UnityEngine;

    public class CharacterEquipment : MonoBehaviour
    {
        [SerializeField] private Weapon[] rightWeapons = new Weapon[3];
        [SerializeField] private Weapon[] leftWeapons = new Weapon[3];
        [SerializeField] private Arrow[] arrows = new Arrow[2];
        [SerializeField] private Spell[] spells = new Spell[5];
        [SerializeField] private Consumable[] consumables = new Consumable[10];

        [SerializeField] private Headgear headgear;
        [SerializeField] private Armor armor;
        [SerializeField] private Boot boots;

        [Header("Components")]
        public CharacterInventory characterInventory;

        private void Awake()
        {
            SyncInventoryWithEquipment();

            SyncDefaultEquipment();
        }

        void SyncInventoryWithEquipment()
        {
            foreach (Weapon weapon in rightWeapons.Where(item => item != null))
                characterInventory.AddItem(weapon, 1);
            foreach (Weapon weapon in leftWeapons.Where(item => item != null))
                characterInventory.AddItem(weapon, 1);
            foreach (Arrow arrow in arrows.Where(item => item != null))
                characterInventory.AddItem(arrow, 1);
            foreach (Spell spell in spells.Where(item => item != null))
                characterInventory.AddItem(spell, 1);
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
    }
}
