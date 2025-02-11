namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using AF;
    using UnityEngine;

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

        public void UpdateClips(Dictionary<string, ActionClip> animationUpdates)
        {
            SetupAnimatorOverrides();

            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            animationEventDispatcher.ClearAll();
            foreach (var kvp in animationUpdates)
            {
                string animationName = kvp.Key;
                ActionClip actionClip = kvp.Value;

                if (actionClip == null)
                {
                    Debug.LogWarning($"Provided animation clip for '{animationName}' is null. Skipping update.");
                    continue;
                }

                clipOverrides[animationName] = actionClip.animationClip;

                foreach (var actionClipEvent in actionClip.Events)
                {
                    string eventName = $"{animationName}_{actionClipEvent.triggerTime}";

                    // Register event in dispatcher
                    animationEventDispatcher.RegisterEvent(eventName, actionClipEvent.OnEvent);

                    // Create and assign AnimationEvent
                    var animationEvent = new UnityEngine.AnimationEvent
                    {
                        functionName = "TriggerEvent",
                        stringParameter = eventName,
                        time = actionClipEvent.triggerTime
                    };

                    clipOverrides[animationName].AddEvent(animationEvent);
                }
            }

            var filtered = clipOverrides
            .Where(clipOverride => clipOverride.Value != null)
            .ToDictionary(clipOverride => clipOverride.Key, clipOverride => clipOverride.Value);

            animatorOverrideController.ApplyOverrides(filtered.ToList());

            animator.runtimeAnimatorController = animatorOverrideController;
        }
    }
}
