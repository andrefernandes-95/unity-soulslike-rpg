namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Animator))]
    public class AnimatorOverrideManager : MonoBehaviour
    {
        Animator animator => GetComponent<Animator>();
        AnimatorOverrideController animatorOverrideController;

        [SerializeField] AnimationEventDispatcher animationEventDispatcher;

        void SetupAnimatorOverrides()
        {
            if (animatorOverrideController == null)
                animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        }

        AnimationClipOverrides GetOverrides()
        {
            SetupAnimatorOverrides();

            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            return clipOverrides;
        }

        public void UpdateCommonClips(Dictionary<string, ActionClip> animationUpdates)
        {
            UpdateAnimationClips(animationUpdates, animationEventDispatcher.commonActions);
        }

        public void UpdateLeftHandClips(Dictionary<string, ActionClip> animationUpdates)
        {
            UpdateAnimationClips(animationUpdates, animationEventDispatcher.leftHandActions);
        }

        private void UpdateAnimationClips(Dictionary<string, ActionClip> animationUpdates, Dictionary<string, UnityEvent> eventList)
        {
            eventList.Clear();

            // Get the original overrides and filter them
            var filteredOverrides = GetOverrides()
                .Where(kvp =>
                {
                    AnimationClip overrideEntry = kvp.Key;
                    string overrideName = overrideEntry.name;
                    return animationUpdates.ContainsKey(overrideName);
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


            Dictionary<AnimationClip, AnimationClip> overrides = new();

            foreach (var kvp in filteredOverrides)
            {
                string animationName = kvp.Key.name;
                if (!animationUpdates.ContainsKey(animationName))
                {
                    continue;
                }

                ActionClip actionClip = animationUpdates[animationName];

                if (actionClip == null)
                {
                    Debug.LogWarning($"Provided animation clip for '{animationName}' is null. Skipping update.");
                    continue;
                }

                overrides[kvp.Key] = actionClip.animationClip;

                foreach (var actionClipEvent in actionClip.Events)
                {
                    UnityEngine.AnimationEvent animationEvent = animationEventDispatcher.RegisterEvent(animationName, actionClipEvent, eventList, actionClip.animationClip.length);
                    overrides[kvp.Key].AddEvent(animationEvent);
                }
            }

            SetOverrides(overrides);
        }

        public void SetOverrides(Dictionary<AnimationClip, AnimationClip> clipOverrides)
        {
            var filtered = clipOverrides
            .Where(clipOverride => clipOverride.Value != null)
            .ToDictionary(clipOverride => clipOverride.Key, clipOverride => clipOverride.Value);

            animatorOverrideController.ApplyOverrides(filtered.ToList());

            animator.runtimeAnimatorController = animatorOverrideController;
        }
    }
}
