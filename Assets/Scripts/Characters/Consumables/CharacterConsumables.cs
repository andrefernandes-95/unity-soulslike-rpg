namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterConsumables : MonoBehaviour
    {
        public ConsumableInstance[] consumables = new ConsumableInstance[6];
        [SerializeField] int activeConsumableIndex = 0;

        public UnityEvent onConsumableSwitched = new();

        public void EquipConsumable(ConsumableInstance consumableInstance, int slot)
        {
            bool shouldUnequip = consumables[slot] == consumableInstance;
            UnequipConsumable(slot);

            if (shouldUnequip)
            {
                return;
            }

            consumables[slot] = consumableInstance;
            SwitchConsumable(activeConsumableIndex);
        }

        public void UnequipConsumable(int slot)
        {
            consumables[slot] = null;
            SwitchConsumable(activeConsumableIndex);
        }

        void SwitchConsumable(int newIndex)
        {
            this.activeConsumableIndex = newIndex;
            onConsumableSwitched?.Invoke();
        }

    }
}
