namespace AFV2
{
    using AF;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class EquipmentSlotButton : SlotButton
    {
        [SerializeField] int index = 0;

        [Header("Components")]
        CharacterEquipmentUI characterEquipmentUI => GetComponentInParent<CharacterEquipmentUI>();
        protected Button button => GetComponent<Button>();
        [SerializeField] Sprite unequipped;

        void Start()
        {
            button.onClick.AddListener(() => characterEquipmentUI.OnSlotSelect(equipmentSlotType, index));

            RegisterEvents();
        }

        protected override void OnSelect(BaseEventData eventData)
        {
            if (TryGetSlotItem(out Item item) && !IsFallbackItem(item))
            {
                characterEquipmentUI.UpdateSelectedSlotLabel(item.DisplayName);
                return;
            }

            characterEquipmentUI.UpdateSelectedSlotLabel(GetSlotName());
        }
        protected override void OnDeselect(BaseEventData eventData)
        {
            characterEquipmentUI.UpdateSelectedSlotLabel("");
        }

        void OnEnable()
        {
            if (TryGetSlotItem(out Item item))
            {
                if (IsFallbackItem(item))
                {
                    itemIcon.sprite = unequipped;
                    return;
                }

                itemIcon.sprite = item.Sprite;
                return;
            }

            itemIcon.sprite = unequipped;
        }

        bool TryGetSlotItem(out Item item)
        {
            item = CharacterEquipmentUtils.GetEquippedItemSlot(characterEquipmentUI.characterEquipment, equipmentSlotType, index);
            return item != null;
        }

        bool IsFallbackItem(Item item)
        {
            if (item is Weapon && characterEquipmentUI.characterEquipment.characterWeapons.FallbackWeapon == item)
            {
                return true;
            }

            return false;
        }

        string GetSlotName()
        {
            if (Glossary.IsPortuguese())
            {
                if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
                    return "Mão Direita";
                if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
                    return "Mão Esquerda";
                if (equipmentSlotType == EquipmentSlotType.SKILL)
                    return "Abilidades / Feitiços";
                if (equipmentSlotType == EquipmentSlotType.ARROW)
                    return "Flechas / Projéteis";
                if (equipmentSlotType == EquipmentSlotType.CONSUMABLE)
                    return "Consumíveis";
                if (equipmentSlotType == EquipmentSlotType.ACCESSORY)
                    return "Acessórios";
                if (equipmentSlotType == EquipmentSlotType.HEADGEAR)
                    return "Capacete";
                if (equipmentSlotType == EquipmentSlotType.ARMOR)
                    return "Veste";
                if (equipmentSlotType == EquipmentSlotType.BOOTS)
                    return "Botas";
            }

            if (equipmentSlotType == EquipmentSlotType.RIGHT_HAND)
                return "Right Hand";
            if (equipmentSlotType == EquipmentSlotType.LEFT_HAND)
                return "Left Hand";
            if (equipmentSlotType == EquipmentSlotType.SKILL)
                return "Skills / Spells";
            if (equipmentSlotType == EquipmentSlotType.ARROW)
                return "Arrows / Throwables";
            if (equipmentSlotType == EquipmentSlotType.CONSUMABLE)
                return "Consumables";
            if (equipmentSlotType == EquipmentSlotType.ACCESSORY)
                return "Accessories";
            if (equipmentSlotType == EquipmentSlotType.HEADGEAR)
                return "Headgear";
            if (equipmentSlotType == EquipmentSlotType.ARMOR)
                return "Armor";
            if (equipmentSlotType == EquipmentSlotType.BOOTS)
                return "Boots";

            return "";
        }
    }
}
