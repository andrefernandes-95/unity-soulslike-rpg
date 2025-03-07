namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterSkills : MonoBehaviour
    {
        public SkillInstance[] skills = new SkillInstance[6];
        [SerializeField] int activeSkillIndex = 0;

        public UnityEvent onSkillSwitched = new();

        public void EquipSkill(SkillInstance skillInstance, int slot)
        {
            bool shouldUnequip = skills[slot] == skillInstance;
            UnequipSkill(slot);

            if (shouldUnequip)
            {
                return;
            }

            skills[slot] = skillInstance;
            onSkillSwitched?.Invoke();
        }

        public void UnequipSkill(int slot = 0)
        {
            skills[slot] = null;
            onSkillSwitched?.Invoke();
        }

        public bool TryGetSkillInstance(out SkillInstance skillInstance)
        {
            skillInstance = skills[activeSkillIndex];

            return skillInstance != null;
        }
    }
}
