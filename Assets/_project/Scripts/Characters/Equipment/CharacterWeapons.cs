namespace AFV2
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterWeapons : MonoBehaviour
    {
        [SerializeField] int activeLeftWeaponIndex = 0;
        [SerializeField] int activeRightWeaponIndex = 0;

        [SerializeField] Weapon fallbackWeapon;
        public Weapon FallbackWeapon => fallbackWeapon;

        [SerializeField] private Weapon[] rightWeapons = new Weapon[3];
        public Weapon[] RightWeapons => rightWeapons;
        [SerializeField] private Weapon[] leftWeapons = new Weapon[3];
        public Weapon[] LeftWeapons => leftWeapons;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        [Header("Flags")]
        [SerializeField] bool isTwoHanding = false;
        public bool IsTwoHanding => isTwoHanding;

        public WeaponInstance CurrentLeftWeaponInstance;
        public WeaponInstance CurrentRightWeaponInstance;

        // Events
        WeaponInstance[] allWeaponInstances => characterApi.GetComponentsInChildren<WeaponInstance>(true);

        UnityAction<EquipmentSlotType, Weapon> onWeaponSwitched;

        private void Start()
        {
            SyncInventoryWithEquipment();

            AssignListeners();
        }

        void SyncInventoryWithEquipment()
        {
            foreach (Weapon weapon in rightWeapons.Where(item => item != null))
                characterApi.characterInventory.AddItem(weapon, 1);
            foreach (Weapon weapon in leftWeapons.Where(item => item != null))
                characterApi.characterInventory.AddItem(weapon, 1);
        }

        void AssignListeners()
        {
            foreach (var weaponInstance in allWeaponInstances)
            {
                onWeaponSwitched += weaponInstance.OnWeaponSwitched;
            }
        }

        public void EquipRightWeapon(Weapon weapon, int slot)
        {
            bool isSameWeapon = rightWeapons[slot] == weapon;

            if (isSameWeapon)
            {
                UnequipRightWeapon(slot);
                return;
            }

            rightWeapons[slot] = weapon;

            SwitchRightWeapon(slot);
        }

        public void EquipLeftWeapon(Weapon weapon, int slot)
        {
            bool isSameWeapon = leftWeapons[slot] == weapon;

            if (isSameWeapon)
            {
                UnequipLeftWeapon(slot);
                return;
            }

            leftWeapons[slot] = weapon;

            SwitchLeftWeapon(slot);
        }

        public void UnequipRightWeapon(int slot)
        {
            rightWeapons[slot] = fallbackWeapon;

            SwitchRightWeapon(slot);
        }

        public void UnequipLeftWeapon(int slot)
        {
            leftWeapons[slot] = fallbackWeapon;

            SwitchLeftWeapon(slot);
        }

        void SwitchRightWeapon(int newIndex)
        {
            this.activeRightWeaponIndex = newIndex;

            foreach (var weaponInstance in allWeaponInstances)
            {
                weaponInstance.OnWeaponSwitched(EquipmentSlotType.RIGHT_HAND, rightWeapons[activeRightWeaponIndex]);
            }
        }

        void SwitchLeftWeapon(int newIndex)
        {
            this.activeLeftWeaponIndex = newIndex;

            foreach (var weaponInstance in allWeaponInstances)
            {
                weaponInstance.OnWeaponSwitched(EquipmentSlotType.LEFT_HAND, leftWeapons[activeLeftWeaponIndex]);
            }
        }

        public void ToggleTwoHanding()
        {
            isTwoHanding = !isTwoHanding;

            TryGetActiveRightWeapon(out WeaponInstance rightHandWeapon);
            TryGetActiveLeftWeapon(out WeaponInstance leftHandWeapon);
        }

        public bool TryGetActiveRightWeapon(out WeaponInstance weapon)
        {
            weapon = CurrentRightWeaponInstance;

            return weapon != null;
        }
        public bool TryGetActiveLeftWeapon(out WeaponInstance weapon)
        {
            weapon = CurrentLeftWeaponInstance;

            return weapon != null;
        }

        public bool HasRightAndLeftWeapon() => TryGetActiveRightWeapon(out _) && TryGetActiveRightWeapon(out _);
        public bool IsPowerStancing() =>
            isTwoHanding == false
            && TryGetActiveRightWeapon(out WeaponInstance rightWeapon)
            && TryGetActiveLeftWeapon(out WeaponInstance leftWeapon)
            && rightWeapon == leftWeapon;

        public void EnableLeftWeaponHitbox()
        {
            if (TryGetActiveLeftWeapon(out WeaponInstance weapon)) weapon.EnableHitbox();
        }

        public void DisableLeftWeaponHitbox()
        {
            if (TryGetActiveLeftWeapon(out WeaponInstance weapon)) weapon.DisableHitbox();
        }

        public void EnableRightWeaponHitbox()
        {
            if (TryGetActiveRightWeapon(out WeaponInstance weapon)) weapon.EnableHitbox();
        }

        public void DisableRightWeaponHitbox()
        {
            if (TryGetActiveRightWeapon(out WeaponInstance weapon)) weapon.DisableHitbox();
        }

        public void DisableAllHitboxes()
        {
            DisableLeftWeaponHitbox();
            DisableRightWeaponHitbox();
        }

        public bool IsFallbackWeapon(Weapon weapon)
        {
            if (weapon == null)
            {
                return false;
            }

            if (FallbackWeapon == null)
            {
                return false;
            }

            return weapon.name == FallbackWeapon.name;
        }
    }
}
