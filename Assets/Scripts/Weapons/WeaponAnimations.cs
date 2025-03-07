namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public class WeaponAnimations : MonoBehaviour
    {

        [Header("Animation Clips")]
        public Transform commonActionsContainer;
        List<ActionClip> inheritedActionClips = new();
        public ActionClip[] animationClips => GetComponentsInChildren<ActionClip>();
        private Dictionary<string, ActionClip> animationClipLookup = new();

        [Header("Attack Animations")]
        [SerializeField] private List<string> lightAttackAnimations = new() { "Right Attack A", "Right Attack B" };
        [SerializeField] private List<string> heavyAttackAnimations = new() { "Heavy Attack A" };
        [SerializeField] private List<string> airAttackAnimations = new() { "Right Air Attack A" };

        public List<string> LightAttackAnimations => lightAttackAnimations.ToList();
        public List<string> HeavyAttackAnimations => heavyAttackAnimations.ToList();
        public List<string> AirAttackAnimations => airAttackAnimations.ToList();

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        private void Awake()
        {
            InitializeAnimationClipLookup();
        }

        private void InitializeAnimationClipLookup()
        {
            if (animationClipLookup.Count <= 0)
            {
                if (inheritedActionClips.Count <= 0 && commonActionsContainer != null)
                {
                    inheritedActionClips = commonActionsContainer.GetComponentsInChildren<ActionClip>(true).ToList();
                }

                foreach (var clip in inheritedActionClips)
                {
                    if (!animationClipLookup.ContainsKey(clip.name))
                    {
                        animationClipLookup.Add(clip.name, clip);
                    }
                }

                foreach (var clip in animationClips)
                {
                    if (!animationClipLookup.ContainsKey(clip.name))
                    {
                        animationClipLookup.Add(clip.name, clip);
                    }
                    else
                    {
                        // Override inherited clip
                        animationClipLookup[clip.name] = clip;
                    }
                }
            }
        }

        public async void ApplyAnimations(bool isLeftHand)
        {
            while (characterApi.animatorManager.IsInTransition())
            {
                await Task.Yield();
            }

            InitializeAnimationClipLookup();

            if (isLeftHand)
            {
                characterApi.animatorManager.animatorOverrideManager.UpdateLeftHandClips(animationClipLookup);
            }
            else
            {
                characterApi.animatorManager.animatorOverrideManager.UpdateCommonClips(animationClipLookup);
            }
        }
    }
}
