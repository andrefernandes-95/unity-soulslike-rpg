namespace AFV2
{
    using System.Linq;
    using UnityEngine;

    public class CharacterWeapons : MonoBehaviour
    {
        [SerializeField] int activeLeftWeaponIndex = 0;
        [SerializeField] int activRightWeaponIndex = 0;

        [SerializeField] Weapon fallbackWeapon;
        public Weapon FallbackWeapon => fallbackWeapon;

        [SerializeField] private Weapon[] rightWeapons = new Weapon[3];
        public Weapon[] RightWeapons => rightWeapons;
        [SerializeField] private Weapon[] leftWeapons = new Weapon[3];
        public Weapon[] LeftWeapons => leftWeapons;

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

        public void EquipRightWeapon(Weapon weapon, int slot)
        {
            UnequipRightWeapon(slot);

            rightWeapons[slot] = weapon;

            if (activRightWeaponIndex == slot)
                weapon.Equip(rightHand, true);
        }

        public void EquipLeftWeapon(Weapon weapon, int slot)
        {
            UnequipLeftWeapon(slot);

            leftWeapons[slot] = weapon;

            if (activeLeftWeaponIndex == slot)
                weapon.Equip(leftHand, false);
        }

        public void UnequipRightWeapon(int slot)
        {
            if (rightWeapons[slot] == null) return;

            rightWeapons[slot].Unequip(rightHand);
            rightWeapons[slot] = null;
        }

        public void UnequipLeftWeapon(int slot)
        {
            if (leftWeapons[slot] == null) return;

            leftWeapons[slot].Unequip(leftHand);
            leftWeapons[slot] = null;
        }

        public void ToggleTwoHanding()
        {
            isTwoHanding = !isTwoHanding;

            TryGetActiveRightWeapon(out Weapon rightHandWeapon);
            TryGetActiveLeftWeapon(out Weapon leftHandWeapon);

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

        public bool TryGetActiveRightWeapon(out Weapon weapon)
        {
            weapon = rightHand.GetComponentInChildren<Weapon>();

            return weapon != null;
        }
        public bool TryGetActiveLeftWeapon(out Weapon weapon)
        {
            weapon = leftHand.GetComponentInChildren<Weapon>();

            return weapon != null;
        }

        public bool HasRightAndLeftWeapon() => TryGetActiveRightWeapon(out _) && TryGetActiveRightWeapon(out _);
        public bool IsPowerStancing() =>
            isTwoHanding == false
            && TryGetActiveRightWeapon(out Weapon rightWeapon)
            && TryGetActiveLeftWeapon(out Weapon leftWeapon)
            && rightWeapon == leftWeapon;

        public void EnableLeftWeaponHitbox()
        {
            if (TryGetActiveLeftWeapon(out Weapon weapon)) weapon.EnableHitbox();
        }

        public void DisableLeftWeaponHitbox()
        {
            if (TryGetActiveLeftWeapon(out Weapon weapon)) weapon.DisableHitbox();
        }

        public void EnableRightWeaponHitbox()
        {
            if (TryGetActiveRightWeapon(out Weapon weapon)) weapon.EnableHitbox();
        }

        public void DisableRightWeaponHitbox()
        {
            if (TryGetActiveRightWeapon(out Weapon weapon)) weapon.DisableHitbox();
        }

        public void DisableAllHitboxes()
        {
            DisableLeftWeaponHitbox();
            DisableRightWeaponHitbox();
        }

    }
}
