namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Events;

    public class CharacterArchery : MonoBehaviour
    {

        [Header("🏹 Arrows")]
        public Arrow[] arrows = new Arrow[2];
        [SerializeField] int activeArrowIndex = 0;

        public UnityEvent<Arrow> onArrowSwitched = new();
        public UnityEvent onArrowRemoved = new();

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        [Header("Look At Constraints")]
        [SerializeField] LookAtConstraint lookAtConstraint;

        // Events
        ArrowWorld[] allArrowWorldInstances => characterApi.GetComponentsInChildren<ArrowWorld>(true);
        Dictionary<Arrow, ArrowWorld> arrowWorldDictionary = new();

        [Header("Events")]
        public UnityEvent onShotFinished;

        void Awake()
        {
            foreach (ArrowWorld arrowWorld in allArrowWorldInstances)
            {
                onArrowSwitched.AddListener((Arrow arrow) =>
                {
                    arrowWorld.OnArrowSwitched(arrow);
                });

                if (!arrowWorldDictionary.ContainsKey(arrowWorld.arrow))
                {
                    arrowWorldDictionary.Add(arrowWorld.arrow, arrowWorld);
                }
            }

            SwitchArrow(activeArrowIndex);
        }

        public void EquipArrow(ArrowInstance arrowInstance, int slot)
        {
            bool shouldUnequip = arrows[slot] == arrowInstance.item;
            UnequipArrow(slot);

            if (shouldUnequip)
            {
                return;
            }

            arrows[slot] = arrowInstance.item as Arrow;

            SwitchArrow(activeArrowIndex);

            characterApi.characterEquipment.onEquipmentChange?.Invoke();
        }

        public void UnequipArrow(int slot)
        {
            arrows[slot] = null;

            SwitchArrow(activeArrowIndex);

            characterApi.characterEquipment.onEquipmentChange?.Invoke();
        }

        public void SwitchArrow()
        {
            var newIndex = activeArrowIndex + 1;

            if (newIndex >= arrows.Length)
            {
                newIndex = 0;
            }

            SwitchArrow(newIndex);
        }

        void SwitchArrow(int index)
        {
            this.activeArrowIndex = index;
            onArrowSwitched?.Invoke(arrows[activeArrowIndex]);
        }

        public void Shoot()
        {
            Arrow currentArrow = arrows[activeArrowIndex];

            if (!arrowWorldDictionary.ContainsKey(currentArrow))
            {
                return;
            }

            if (!characterApi.characterWeapons.HasRangeWeapon())
            {
                return;
            }

            WorldWeapon bowWeapon = characterApi.characterWeapons.CurrentRightWeaponInstance;
            arrowWorldDictionary[currentArrow].Shoot(bowWeapon.transform.position, lookAtConstraint.GetSource(0).sourceTransform.transform.rotation);

            RemoveArrowFromInventory(currentArrow);
            onArrowRemoved?.Invoke();
        }

        void RemoveArrowFromInventory(Arrow arrow)
        {
            if (!characterApi.characterInventory.OwnedItems.ContainsKey(arrow))
            {
                UnequipArrow(activeArrowIndex);

                return;
            }

            List<ItemInstance> arrowInstances = characterApi.characterInventory.OwnedItems[arrow];

            if (arrowInstances.Count > 0)
            {
                characterApi.characterInventory.RemoveItem(arrowInstances[0]);
            }

            if (arrowInstances.Count == 0)
            {
                UnequipArrow(activeArrowIndex);
            }
        }

        void HandleArrowWorld(bool show)
        {
            Arrow currentArrow = arrows[activeArrowIndex];
            if (currentArrow == null)
            {
                return;
            }

            if (!arrowWorldDictionary.ContainsKey(currentArrow))
            {
                return;
            }
            arrowWorldDictionary[currentArrow].gameObject.SetActive(show);

        }

        public void ShowArrowWorld()
        {
            HandleArrowWorld(true);
        }
        public void HideArrowWorld()
        {
            HandleArrowWorld(false);
        }

        public Arrow GetCurrentArrow()
        {
            return arrows[activeArrowIndex];
        }
    }
}
