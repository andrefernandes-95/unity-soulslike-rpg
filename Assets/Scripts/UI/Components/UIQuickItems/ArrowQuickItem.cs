namespace AFV2
{
    using UnityEngine;

    public class ArrowQuickItem : QuickItem
    {
        [SerializeField] Sprite unequippedArrowIcon;

        new void Awake()
        {
            base.Awake();

            characterApi.characterArchery.onArrowSwitched.AddListener(OnArrowSwitched);
            characterApi.characterEquipment.onEquipmentChange.AddListener(HandleEquipmentChange);
            characterApi.characterArchery.onArrowRemoved.AddListener(HandleEquipmentChange);
        }

        void HandleEquipmentChange()
        {
            if (characterApi.characterWeapons.HasRangeWeapon())
            {
                Arrow arrow = characterApi.characterArchery.GetCurrentArrow();
                OnArrowSwitched(arrow);
            }
        }

        void OnArrowSwitched(Arrow arrow)
        {
            HideItemCount();

            if (arrow != null)
            {
                UpdateIcon(arrow.Sprite);
                ShowItemCount(arrow);
            }
            else
            {
                UpdateIcon(unequippedArrowIcon);
            }
        }
    }
}
