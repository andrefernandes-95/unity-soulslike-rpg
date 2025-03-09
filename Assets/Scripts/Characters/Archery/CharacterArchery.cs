namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Events;

    public class CharacterArchery : MonoBehaviour
    {

        [Header("🏹 Arrows")]
        public Arrow[] arrows = new Arrow[2];
        [SerializeField] int activeArrowIndex = 0;

        public UnityEvent<Arrow> onArrowSwitched = new();

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
        }

        public void UnequipArrow(int slot)
        {
            arrows[slot] = null;

            SwitchArrow(activeArrowIndex);
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
        }

        void HandleArrowWorld(bool show)
        {
            Arrow currentArrow = arrows[activeArrowIndex];

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
    }
}
