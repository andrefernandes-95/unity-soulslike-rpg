namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterArchery : MonoBehaviour
    {

        [Header("🏹 Arrows")]
        public ArrowInstance[] arrows = new ArrowInstance[2];
        [SerializeField] int activeArrowIndex = 0;

        public UnityEvent onArrowSwitched = new();

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

        public bool TryGetArrowInstance(out ArrowInstance arrowInstance)
        {
            arrowInstance = arrows[activeArrowIndex];

            return arrowInstance != null;
        }
    }
}
