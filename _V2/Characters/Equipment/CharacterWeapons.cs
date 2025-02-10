namespace AFV2
{
    using System.Linq;
    using UnityEngine;

    public class CharacterWeapons : MonoBehaviour
    {
        [SerializeField] private Weapon[] rightWeapons = new Weapon[3];
        [SerializeField] private Weapon[] leftWeapons = new Weapon[3];

        [Header("Body Parts")]
        public Transform rightHand;
        public Transform leftHand;

        [Header("Components")]
        public CharacterInventory characterInventory;

        [Header("Flags")]
        [SerializeField] bool isTwoHanding = false;
        public bool IsTwoHanding => isTwoHanding;

        private void Awake()
        {
            SyncInventoryWithEquipment();
        }

        void SyncInventoryWithEquipment()
        {
            foreach (Weapon weapon in rightWeapons.Where(item => item != null))
                characterInventory.AddItem(weapon, 1);
            foreach (Weapon weapon in leftWeapons.Where(item => item != null))
                characterInventory.AddItem(weapon, 1);
        }

        public void EquipRightWeapon(Weapon weapon, int slot = 0)
        {
            rightWeapons[slot] = weapon;

            weapon.Equip(rightHand, true);
        }

        public void EquipLeftWeapon(Weapon weapon, int slot = 0)
        {
            leftWeapons[slot] = weapon;
            weapon.Equip(leftHand, false);
        }

        public void UnequipRightWeapon(int slot = 0)
        {
            if (rightWeapons[slot] == null) return;

            rightWeapons[slot].Unequip(rightHand);
            rightWeapons[slot] = null;

        }
        public void UnequipLeftWeapon(int slot = 0)
        {
            if (leftWeapons[slot] == null) return;

            leftWeapons[slot].Unequip(leftHand);
            leftWeapons[slot] = null;
        }

        public void ToggleTwoHanding()
        {
            isTwoHanding = !isTwoHanding;

            Weapon rightHandWeapon = GetActiveRightWeapon();
            Weapon leftHandWeapon = GetActiveLeftWeapon();

            if (isTwoHanding)
            {
                rightHandWeapon?.ApplyPivots(rightHandWeapon, true);
                rightHandWeapon?.ApplyAnimations(rightHandWeapon);
                leftHandWeapon?.gameObject.SetActive(false);
            }
            else
            {
                rightHandWeapon?.ApplyPivots(rightHandWeapon, true);
                rightHandWeapon?.ApplyAnimations(rightHandWeapon);

                leftHandWeapon?.gameObject.SetActive(true);
                leftHandWeapon?.ApplyPivots(leftHandWeapon, false);
                leftHandWeapon?.ApplyAnimations(leftHandWeapon);
            }
        }

        Weapon GetActiveRightWeapon()
        {
            return rightHand.GetComponentInChildren<Weapon>();
        }
        Weapon GetActiveLeftWeapon()
        {
            return leftHand.GetComponentInChildren<Weapon>();
        }
    }
}
