namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public class LeftWeaponAnimations : MonoBehaviour
    {

        [Header("Animation Clips")]
        public ActionClip[] animationClips => GetComponentsInChildren<ActionClip>(true);
        private Dictionary<string, ActionClip> animationClipLookup = new();

        [Header("Attack Animations")]
        [SerializeField] private List<string> lightAttackAnimations = new() { "Left Attack A", "Left Attack B" };
        [SerializeField] private List<string> airAttackAnimations = new() { "Left Air Attack A" };

        public List<string> LightAttackAnimations => lightAttackAnimations.ToList();
        public List<string> AirAttackAnimations => airAttackAnimations.ToList();

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        private void Awake()
        {
            InitializeAnimationClipLookup();
        }

        private void InitializeAnimationClipLookup()
        {
            foreach (var clip in animationClips)
            {
                if (!animationClipLookup.ContainsKey(clip.name))
                {
                    animationClipLookup.Add(clip.name, clip);
                }
                else
                {
                    Debug.LogWarning($"Duplicate animation clip name found: {clip.name}");
                }
            }
        }

        public async void ApplyAnimations()
        {
            while (characterApi.animatorManager.IsInTransition())
            {
                await Task.Yield();
            }

            characterApi.animatorManager.animatorOverrideManager.UpdateLeftHandClips(animationClipLookup);
        }

        public void OnWeaponSwitched(EquipmentSlotType slot, Item item, Weapon equippedWeapon)
        {
            if (slot == EquipmentSlotType.LEFT_HAND)
            {
                Weapon weapon = item as Weapon;

                if (weapon == equippedWeapon)
                {
                    ApplyAnimations();
                }
            }
        }
    }
}
