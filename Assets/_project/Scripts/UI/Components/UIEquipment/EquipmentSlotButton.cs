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
            if (TryGetSlotItem(out Item item) && !IsFallbackItem(item))
            {
                uICharacterEquipment.UpdateSelectedSlotLabel(item.DisplayName);
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
            if (TryGetSlotItem(out Item item))
            {
                if (IsFallbackItem(item))
                {
                    backgroundIcon.sprite = unequipped.sprite;
                    return;
                }

                backgroundIcon.sprite = item.Sprite;
                return;
            }

            backgroundIcon.sprite = unequipped.sprite;
        }

        bool TryGetSlotItem(out Item item)
        {
            item = GetEquippedItemSlot(uICharacterEquipment.characterEquipment, slotType, slotIndex);
            return item != null;
        }

        bool IsFallbackItem(Item item)
        {
            if (item is Weapon && uICharacterEquipment.characterEquipment.characterWeapons.FallbackWeapon == item)
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


        Item GetEquippedItemSlot(CharacterEquipment characterEquipment, EquipmentSlotType equipmentSlotType, int slotIndex)
        {
            if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
                return characterEquipment.characterWeapons.RightWeapons[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
                return characterEquipment.characterWeapons.LeftWeapons[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.ARROW)
                return characterEquipment.Arrows[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.SKILL)
                return characterEquipment.Skills[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.ACCESSORY)
                return characterEquipment.Accessories[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.CONSUMABLE)
                return characterEquipment.Consumables[slotIndex];
            if (equipmentSlotType == EquipmentSlotType.HEADGEAR)
                return characterEquipment.Headgear;
            if (equipmentSlotType == EquipmentSlotType.ARMOR)
                return characterEquipment.Armor;
            if (equipmentSlotType == EquipmentSlotType.BOOTS)
                return characterEquipment.Boots;

            return null;

        }
    }
}
