namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterWeapons : MonoBehaviour
    {
        [Header("⚔️ Weapons")]
        public WeaponInstance[] rightWeapons = new WeaponInstance[3];
        public WeaponInstance[] leftWeapons = new WeaponInstance[3];
        [SerializeField] int activeLeftWeaponIndex = 0;
        [SerializeField] int activeRightWeaponIndex = 0;

        [Header("Feet Weapons")]

        public WorldWeapon CurrentLeftFootWeaponInstance;
        public WorldWeapon CurrentRightFootWeaponInstance;


        [Header("Components")]
        [SerializeField] CharacterApi characterApi;
        [SerializeField] CharacterDefaultEquipment characterDefaultEquipment;

        [Header("Flags")]
        [SerializeField] bool isTwoHanding = false;
        public bool IsTwoHanding => isTwoHanding;

        public WorldWeapon CurrentLeftWeaponInstance;
        public WorldWeapon CurrentRightWeaponInstance;

        // Events
        WorldWeapon[] allWeaponInstances => characterApi.GetComponentsInChildren<WorldWeapon>(true);

        public UnityEvent<WeaponInstance> onRightWeaponSwitched = new();
        public UnityEvent<WeaponInstance> onLeftWeaponSwitched = new();

        void Awake()
        {
            foreach (WorldWeapon worldWeapon in allWeaponInstances)
            {
                onRightWeaponSwitched.AddListener((WeaponInstance weaponInstance) =>
                {
                    worldWeapon.OnWeaponSwitched(EquipmentSlotType.RIGHT_HAND, weaponInstance);
                });

                onLeftWeaponSwitched.AddListener((WeaponInstance weaponInstance) =>
                {
                    worldWeapon.OnWeaponSwitched(EquipmentSlotType.LEFT_HAND, weaponInstance);
                });
            }

            SwitchRightWeapon(0);
            SwitchLeftWeapon(0);
        }

        public void EquipRightWeapon(WeaponInstance weaponInstance, int slot)
        {
            bool isSameWeapon = rightWeapons[slot] == weaponInstance;

            if (isSameWeapon)
            {
                UnequipRightWeapon(slot);
                return;
            }

            rightWeapons[slot] = weaponInstance;
            SwitchRightWeapon(slot);
        }

        public void EquipLeftWeapon(WeaponInstance weaponInstance, int slot)
        {
            bool isSameWeapon = leftWeapons[slot] == weaponInstance;

            if (isSameWeapon)
            {
                UnequipLeftWeapon(slot);
                return;
            }

            leftWeapons[slot] = weaponInstance;
            SwitchLeftWeapon(slot);
        }

        public void UnequipRightWeapon(int slot)
        {
            WeaponInstance unarmedWeapon = new(characterDefaultEquipment.fallbackWeapon);
            rightWeapons[slot] = unarmedWeapon;
            SwitchRightWeapon(slot);
        }

        public void UnequipLeftWeapon(int slot)
        {
            WeaponInstance unarmedWeapon = new(characterDefaultEquipment.fallbackWeapon);
            leftWeapons[slot] = unarmedWeapon;
            SwitchLeftWeapon(slot);
        }

        public void SwitchRightWeapon()
        {
            var newIndex = activeRightWeaponIndex + 1;

            if (newIndex >= rightWeapons.Length)
            {
                newIndex = 0;
            }

            SwitchRightWeapon(newIndex);
        }

        public void SwitchLeftWeapon()
        {
            var newIndex = activeLeftWeaponIndex + 1;

            if (newIndex >= leftWeapons.Length)
            {
                newIndex = 0;
            }

            SwitchLeftWeapon(newIndex);
        }

        void SwitchRightWeapon(int newIndex)
        {
            this.activeRightWeaponIndex = newIndex;

            onRightWeaponSwitched?.Invoke(rightWeapons[activeRightWeaponIndex]);
        }

        void SwitchLeftWeapon(int newIndex)
        {
            this.activeLeftWeaponIndex = newIndex;

            onLeftWeaponSwitched?.Invoke(leftWeapons[activeLeftWeaponIndex]);
        }


        #region Two-Handing
        public void ToggleTwoHanding()
        {
            isTwoHanding = !isTwoHanding;

            TryGetActiveRightWeapon(out WorldWeapon rightHandWeapon);
            TryGetActiveLeftWeapon(out WorldWeapon leftHandWeapon);
        }
        #endregion

        public bool TryGetActiveRightWeapon(out WorldWeapon weapon)
        {
            weapon = CurrentRightWeaponInstance;

            return weapon != null;
        }

        public bool TryGetRightWeaponInstance(out WeaponInstance weaponInstance)
        {
            weaponInstance = rightWeapons[activeRightWeaponIndex];

            return weaponInstance != null;
        }

        public bool TryGetActiveLeftWeapon(out WorldWeapon weapon)
        {
            weapon = CurrentLeftWeaponInstance;

            return weapon != null;
        }

        public bool TryGetLeftWeaponInstance(out WeaponInstance weaponInstance)
        {
            weaponInstance = leftWeapons[activeLeftWeaponIndex];

            return weaponInstance != null;
        }

        public bool HasRightAndLeftWeapon() => TryGetActiveRightWeapon(out _) && TryGetActiveRightWeapon(out _);
        public bool IsPowerStancing() =>
            isTwoHanding == false
            && HasRightAndLeftWeapon()
            && CurrentLeftWeaponInstance == CurrentRightWeaponInstance;

        #region Hitboxes
        public void EnableLeftWeaponHitbox()
        {
            if (TryGetActiveLeftWeapon(out WorldWeapon weapon)) weapon.EnableHitbox();
        }

        public void DisableLeftWeaponHitbox()
        {
            if (TryGetActiveLeftWeapon(out WorldWeapon weapon)) weapon.DisableHitbox();
        }

        public void EnableRightWeaponHitbox()
        {
            if (TryGetActiveRightWeapon(out WorldWeapon weapon)) weapon.EnableHitbox();
        }

        public void DisableRightWeaponHitbox()
        {
            if (TryGetActiveRightWeapon(out WorldWeapon weapon)) weapon.DisableHitbox();
        }

        public void EnableLeftFootHitbox()
        {
            if (CurrentLeftFootWeaponInstance != null)
            {
                CurrentLeftFootWeaponInstance.EnableHitbox();
            }
        }

        public void EnableRightFootHitbox()
        {
            if (CurrentRightFootWeaponInstance != null)
            {
                CurrentRightFootWeaponInstance.EnableHitbox();
            }
        }

        void DisableLeftFootHitbox()
        {
            if (CurrentLeftFootWeaponInstance != null)
            {
                CurrentLeftFootWeaponInstance.DisableHitbox();
            }
        }
        void DisableRightFootHitbox()
        {
            if (CurrentRightFootWeaponInstance != null)
            {
                CurrentRightFootWeaponInstance.DisableHitbox();
            }
        }


        public void DisableAllHitboxes()
        {
            DisableLeftWeaponHitbox();
            DisableRightWeaponHitbox();
            DisableLeftFootHitbox();
            DisableRightFootHitbox();
        }
        #endregion

    }
}
