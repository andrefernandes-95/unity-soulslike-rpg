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

        }

        void OnRightWeaponSwitched(WeaponInstance weaponInstance)
        {
            if (weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
            {
                UpdateIcon(weaponInstance);
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
                UpdateIcon(weaponInstance);
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
                UpdateIcon(skillInstance);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void OnArrowSwitched()
        {
            if (characterApi.characterArchery.TryGetArrowInstance(out ArrowInstance arrowInstance))
            {
                UpdateIcon(arrowInstance);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void UpdateIcon(ItemInstance equippedItem)
        {
            equippedItemIcon.sprite = equippedItem.item.Sprite;
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
