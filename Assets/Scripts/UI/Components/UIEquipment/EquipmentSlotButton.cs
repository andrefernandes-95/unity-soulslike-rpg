namespace AFV2
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(ButtonEnhancer))]
    public class EquipmentSlotButton : MonoBehaviour
    {
        [Header("Slot Settings")]
        [SerializeField] EquipmentSlotType slotType;
        [SerializeField] int slotIndex = 0;

        [Header("Components")]
        UICharacterEquipment uICharacterEquipment => GetComponentInParent<UICharacterEquipment>();
        ButtonEnhancer buttonEnhancer => GetComponent<ButtonEnhancer>();

        [Header("UI")]
        [SerializeField] Image backgroundIcon;

        [Header("Sprites")]
        [SerializeField] SpriteContainer unequipped;

        void Awake()
        {
            AssignEventListeners();
        }

        void AssignEventListeners()
        {
            buttonEnhancer.onClick += OnClick;
            buttonEnhancer.onSelect += OnSelect;
            buttonEnhancer.onDeselect += OnDeselect;
        }

        void OnClick(BaseEventData baseEventData)
        {
            uICharacterEquipment.OnSlotSelect(slotType, slotIndex);
        }

        void OnSelect(BaseEventData eventData)
        {
            if (TryGetSlotItem(out ItemInstance itemInstance) && itemInstance.item != null && !IsFallbackItem(itemInstance))
            {
                uICharacterEquipment.UpdateSelectedSlotLabel(itemInstance.item.DisplayName);
                return;
            }

            uICharacterEquipment.UpdateSelectedSlotLabel(GetSlotName());
        }
        void OnDeselect(BaseEventData eventData)
        {
            uICharacterEquipment.UpdateSelectedSlotLabel("");
        }

        void OnEnable()
        {
            if (TryGetSlotItem(out ItemInstance itemInstance) && itemInstance.item != null)
            {
                if (IsFallbackItem(itemInstance))
                {
                    backgroundIcon.sprite = unequipped.sprite;
                    return;
                }

                backgroundIcon.sprite = itemInstance.item.Sprite;
                return;
            }

            backgroundIcon.sprite = unequipped.sprite;
        }

        bool TryGetSlotItem(out ItemInstance itemInstance)
        {
            itemInstance = GetEquippedItemSlot(uICharacterEquipment.characterEquipment, slotType, slotIndex);
            return itemInstance != null;
        }

        bool IsFallbackItem(ItemInstance itemInstance)
        {
            if (itemInstance.item is Weapon weapon && weapon.isFallbackWeapon)
            {
                return true;
            }

            return false;
        }

        string GetSlotName()
        {
            if (Glossary.IsPortuguese())
            {
                if (slotType == EquipmentSlotType.RIGHT_HAND)
                    return "Mão Direita";
                if (slotType == EquipmentSlotType.LEFT_HAND)
                    return "Mão Esquerda";
                if (slotType == EquipmentSlotType.SKILL)
                    return "Abilidades / Feitiços";
                if (slotType == EquipmentSlotType.ARROW)
                    return "Flechas / Projéteis";
                if (slotType == EquipmentSlotType.CONSUMABLE)
                    return "Consumíveis";
                if (slotType == EquipmentSlotType.ACCESSORY)
                    return "Acessórios";
                if (slotType == EquipmentSlotType.HEADGEAR)
                    return "Capacete";
                if (slotType == EquipmentSlotType.ARMOR)
                    return "Veste";
                if (slotType == EquipmentSlotType.BOOTS)
                    return "Botas";
            }

            if (slotType == EquipmentSlotType.RIGHT_HAND)
                return "Right Hand";
            if (slotType == EquipmentSlotType.LEFT_HAND)
                return "Left Hand";
            if (slotType == EquipmentSlotType.SKILL)
                return "Skills / Spells";
            if (slotType == EquipmentSlotType.ARROW)
                return "Arrows / Throwables";
            if (slotType == EquipmentSlotType.CONSUMABLE)
                return "Consumables";
            if (slotType == EquipmentSlotType.ACCESSORY)
                return "Accessories";
            if (slotType == EquipmentSlotType.HEADGEAR)
                return "Headgear";
            if (slotType == EquipmentSlotType.ARMOR)
                return "Armor";
            if (slotType == EquipmentSlotType.BOOTS)
                return "Boots";

            return "";
        }


        ItemInstance GetEquippedItemSlot(CharacterEquipment characterEquipment, EquipmentSlotType equipmentSlotType, int slotIndex)
        {
            if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
                return characterEquipment.characterWeapons.rightWeapons[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
                return characterEquipment.characterWeapons.leftWeapons[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.ARROW)
                return characterEquipment.characterWeapons.arrows[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.SKILL)
                return characterEquipment.characterWeapons.skills[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.ACCESSORY)
                return characterEquipment.accessories[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.CONSUMABLE)
                return characterEquipment.consumables[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.HEADGEAR)
                return characterEquipment.headgear;
            if (equipmentSlotType == EquipmentSlotType.ARMOR)
                return characterEquipment.armor;
            if (equipmentSlotType == EquipmentSlotType.BOOTS)
                return characterEquipment.boot;

            return null;

        }
    }
}
