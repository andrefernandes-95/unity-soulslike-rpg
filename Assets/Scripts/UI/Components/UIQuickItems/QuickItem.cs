namespace AFV2
{
    using UnityEngine;
    using UnityEngine.UI;

    public class QuickItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image equippedItemIcon;
        [SerializeField] Image unequippedItemIcon;
        [SerializeField] CharacterApi characterApi;

        public EquipmentSlotType equipmentSlotType;

        [Header("Skill / Arrow Slots")]
        [SerializeField] Sprite unequippedSkillIcon;
        [SerializeField] Sprite unequippedArrowIcon;


        void OnEnable()
        {
        }

        void Awake()
        {
            DrawUnequippedItemIcon();

            if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
            {
                characterApi.characterWeapons.onRightWeaponSwitched.AddListener(OnRightWeaponSwitched);
            }
            else if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
            {
                characterApi.characterWeapons.onLeftWeaponSwitched.AddListener(OnLeftWeaponSwitched);
            }
            else if (equipmentSlotType == EquipmentSlotType.SKILL)
            {
                characterApi.characterArchery.onArrowSwitched.AddListener(OnArrowSwitched);
                characterApi.characterSkills.onSkillSwitched.AddListener(OnSkillSwitched);
            }

            characterApi.characterWeapons.onEquipmentChange.AddListener(CheckForSkillOrArrowMode);
            characterApi.characterWeapons.onLeftWeaponSwitched.AddListener((_) => CheckForSkillOrArrowMode());
            characterApi.characterWeapons.onRightWeaponSwitched.AddListener((_) => CheckForSkillOrArrowMode());
        }

        void CheckForSkillOrArrowMode()
        {
            if (equipmentSlotType == EquipmentSlotType.SKILL)
            {
                if (characterApi.characterWeapons.HasRangeWeapon())
                {
                    unequippedItemIcon.sprite = unequippedArrowIcon;
                }
                else
                {
                    unequippedItemIcon.sprite = unequippedSkillIcon;
                }
            }
        }

        void OnRightWeaponSwitched(WeaponInstance weaponInstance)
        {
            if (weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
            {
                UpdateIcon(weaponInstance.item.Sprite);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void OnLeftWeaponSwitched(WeaponInstance weaponInstance)
        {
            if (weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
            {
                UpdateIcon(weaponInstance.item.Sprite);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void OnSkillSwitched()
        {
            if (characterApi.characterSkills.TryGetSkillInstance(out SkillInstance skillInstance))
            {
                UpdateIcon(skillInstance.item.Sprite);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void OnArrowSwitched(Arrow arrow)
        {
            if (arrow != null)
            {
                UpdateIcon(arrow.Sprite);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void UpdateIcon(Sprite equippedItemSprite)
        {
            equippedItemIcon.sprite = equippedItemSprite;
            equippedItemIcon.gameObject.SetActive(true);
            unequippedItemIcon.gameObject.SetActive(false);
        }

        void DrawUnequippedItemIcon()
        {
            equippedItemIcon.gameObject.SetActive(false);
            unequippedItemIcon.gameObject.SetActive(true);
        }
    }
}
