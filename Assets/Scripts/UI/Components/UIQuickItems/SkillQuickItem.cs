namespace AFV2
{
    using UnityEngine;

    public class SkillQuickItem : QuickItem
    {
        [SerializeField] Sprite unequippedSkillIcon;

        new void Awake()
        {
            base.Awake();

            characterApi.characterSkills.onSkillSwitched.AddListener(OnSkillSwitched);
            characterApi.characterWeapons.onEquipmentChange.AddListener(HandleEquipmentChange);
        }

        void HandleEquipmentChange()
        {
            if (!characterApi.characterWeapons.HasRangeWeapon())
            {
                OnSkillSwitched();
            }
        }

        void OnSkillSwitched()
        {
            HideItemCount();

            if (characterApi.characterSkills.TryGetSkillInstance(out SkillInstance skillInstance))
            {
                UpdateIcon(skillInstance.item.Sprite);
            }
            else
            {
                UpdateIcon(unequippedSkillIcon);
            }
        }
    }
}
