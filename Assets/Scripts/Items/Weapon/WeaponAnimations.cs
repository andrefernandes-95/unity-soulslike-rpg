namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public class WeaponAnimations : MonoBehaviour
    {
        public List<ActionClip> oneHanding = new();
        Dictionary<string, ActionClip> oneHandingDictionary = new();
        public List<ActionClip> rightAttacks = new();
        Dictionary<string, ActionClip> rightAttacksDictionary = new();
        public List<ActionClip> leftAttacks = new();
        Dictionary<string, ActionClip> leftAttacksDictionary = new();
        public List<ActionClip> twoHanding = new();
        Dictionary<string, ActionClip> twoHandingDictionary = new();

        [Header("Available Animations")]
        [SerializeField] protected List<string> rightLightAttacks = new() { "Right Attack A", "Right Attack B", "Right Attack C" };
        public List<string> RightLightAttacks => rightLightAttacks.ToList();
        [SerializeField] protected List<string> leftLightAttacks = new() { "Left Attack A", "Left Attack B", "Left Attack C" };
        public List<string> LeftLightAttacks => leftLightAttacks.ToList();
        [SerializeField] protected List<string> heavyAttacks = new() { "Heavy Attack A", "Heavy Attack B", "Heavy Attack C" };
        public List<string> HeavyAttacks => heavyAttacks.ToList();
        [SerializeField] protected List<string> rightAirAttacks = new() { "Right Air Attack A" };
        public List<string> RightAirAttacks => rightAirAttacks.ToList();
        [SerializeField] protected List<string> leftAirAttacks = new() { "Left Air Attack A" };
        public List<string> LeftAirAttacks => leftAirAttacks.ToList();

        CharacterApi characterApi;

        void Awake()
        {
            SetupDictionary(oneHanding, oneHandingDictionary);
            SetupDictionary(rightAttacks, rightAttacksDictionary);
            SetupDictionary(leftAttacks, leftAttacksDictionary);
            SetupDictionary(twoHanding, twoHandingDictionary);
        }

        void SetupDictionary(List<ActionClip> list, Dictionary<string, ActionClip> targetDictionary)
        {
            foreach (var item in list)
            {
                targetDictionary.Add(item.name, item);
            }
        }

        private void OnEnable()
        {
            SetupCharacterApi();
        }

        void SetupCharacterApi()
        {
            if (characterApi == null)
                characterApi = GetComponentInParent<CharacterApi>();
        }


        bool IsOneHandAttackActionClip(ActionClip actionClip, out bool isRightHandAttack, out bool isLeftHandAttack)
        {
            isRightHandAttack = IsRightHandAttackActionClip(actionClip);
            isLeftHandAttack = IsLeftHandAttackActionClip(actionClip);

            return isLeftHandAttack || isRightHandAttack;
        }

        bool IsRightHandAttackActionClip(ActionClip actionClip)
        {
            return RightLightAttacks.Contains(actionClip.name) || RightAirAttacks.Contains(actionClip.name);
        }

        bool IsLeftHandAttackActionClip(ActionClip actionClip)
        {
            return LeftLightAttacks.Contains(actionClip.name) || LeftAirAttacks.Contains(actionClip.name);
        }

        public async void ApplyAnimations(Weapon weapon, bool isRightHandWeapon)
        {
            if (characterApi == null)
            {
                SetupCharacterApi();
            }

            while (characterApi.animatorManager.IsInTransition())
                await Task.Yield();

            characterApi.animatorManager.animatorOverrideManager.UpdateCommonClips(oneHandingDictionary);

            if (isRightHandWeapon)
            {
                characterApi.animatorManager.animatorOverrideManager.UpdateCommonClips(rightAttacksDictionary);
            }
            else
            {
                characterApi.animatorManager.animatorOverrideManager.UpdateLeftHandClips(leftAttacksDictionary);
            }
        }
    }
}
