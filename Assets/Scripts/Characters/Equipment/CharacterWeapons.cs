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

        [Header("🏹 Arrows")]
        public ArrowInstance[] arrows = new ArrowInstance[2];
        [SerializeField] int activeArrowIndex = 0;

        [Header("🔥 Skills")]
        public SkillInstance[] skills = new SkillInstance[6];
        [SerializeField] int activeSkillIndex = 0;

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

        public UnityEvent onRightWeaponSwitched = new();
        public UnityEvent onLeftWeaponSwitched = new();
        public UnityEvent onSkillSwitched = new();
        public UnityEvent onArrowSwitched = new();

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
            WeaponInstance unarmedWeapon = new WeaponInstance(characterDefaultEquipment.fallbackWeapon);
            rightWeapons[slot] = unarmedWeapon;
            SwitchRightWeapon(slot);
        }

        public void UnequipLeftWeapon(int slot)
        {
            WeaponInstance unarmedWeapon = new WeaponInstance(characterDefaultEquipment.fallbackWeapon);
            leftWeapons[slot] = unarmedWeapon;
            SwitchLeftWeapon(slot);
        }

        void SwitchRightWeapon(int newIndex)
        {
            this.activeRightWeaponIndex = newIndex;

            foreach (var weaponInstance in allWeaponInstances)
            {
                weaponInstance.OnWeaponSwitched(EquipmentSlotType.RIGHT_HAND, rightWeapons[activeRightWeaponIndex]);
            }

            onRightWeaponSwitched?.Invoke();
        }

        void SwitchLeftWeapon(int newIndex)
        {
            this.activeLeftWeaponIndex = newIndex;

            foreach (var weaponInstance in allWeaponInstances)
            {
                weaponInstance.OnWeaponSwitched(EquipmentSlotType.LEFT_HAND, leftWeapons[activeLeftWeaponIndex]);
            }

            onLeftWeaponSwitched?.Invoke();
        }


        public void EquipSkill(SkillInstance skillInstance, int slot)
        {
            bool shouldUnequip = skills[slot] == skillInstance;
            UnequipSkill(slot);

            if (shouldUnequip)
            {
                return;
            }

            skills[slot] = skillInstance;
            onSkillSwitched?.Invoke();
        }

        public void UnequipSkill(int slot = 0)
        {
            skills[slot] = null;
            onSkillSwitched?.Invoke();
        }

        public void EquipArrow(ArrowInstance arrowInstance, int slot)
        {
            bool shouldUnequip = arrows[slot] == arrowInstance;
            UnequipArrow(slot);
            if (shouldUnequip)
            {
                return;
            }

            arrows[slot] = arrowInstance;
            onArrowSwitched?.Invoke();
        }

        public void UnequipArrow(int slot)
        {
            arrows[slot] = null;
            onArrowSwitched?.Invoke();
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

        public void DisableAllHitboxes()
        {
            DisableLeftWeaponHitbox();
            DisableRightWeaponHitbox();
        }
        #endregion

        #region Skills

        public bool TryGetSkillInstance(out SkillInstance skillInstance)
        {
            skillInstance = skills[activeSkillIndex];

            return skillInstance != null;
        }

        #endregion

        #region Arrows

        public bool TruyGetArrowInstance(out ArrowInstance arrowInstance)
        {
            arrowInstance = arrows[activeArrowIndex];

            return arrowInstance != null;
        }
        #endregion

    }
}
