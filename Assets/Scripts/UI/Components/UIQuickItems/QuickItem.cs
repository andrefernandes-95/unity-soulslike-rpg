namespace AFV2
{
    using UnityEngine;
    using UnityEngine.UI;

    public class QuickItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image equippedItemIcon;
        [SerializeField] Image unequippedItemIcon;
        [SerializeField] CharacterEquipment characterEquipment;
        [SerializeField] CharacterWeapons characterWeapons;

        public EquipmentSlotType equipmentSlotType;

        void OnEnable()
        {
        }

        void Awake()
        {
            DrawUnequippedItemIcon();

            if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
            {
                characterWeapons.onRightWeaponSwitched.AddListener(OnRightWeaponSwitched);
            }
            else if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
            {
                characterWeapons.onLeftWeaponSwitched.AddListener(OnLeftWeaponSwitched);
            }
            else if (equipmentSlotType == EquipmentSlotType.SKILL)
            {
                characterWeapons.onArrowSwitched.AddListener(OnArrowSwitched);
                characterWeapons.onSkillSwitched.AddListener(OnSkillSwitched);
            }

        }

        void OnRightWeaponSwitched()
        {
            if (characterWeapons.TryGetRightWeaponInstance(out WeaponInstance weaponInstance) && weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
            {
                UpdateIcon(weaponInstance);
            }
            else
            {
                DrawUnequippedItemIcon();
            }
        }

        void OnLeftWeaponSwitched()
        {
            if (characterWeapons.TryGetLeftWeaponInstance(out WeaponInstance weaponInstance) && weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
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
            if (characterWeapons.TryGetSkillInstance(out SkillInstance skillInstance))
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
            if (characterWeapons.TruyGetArrowInstance(out ArrowInstance arrowInstance))
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
