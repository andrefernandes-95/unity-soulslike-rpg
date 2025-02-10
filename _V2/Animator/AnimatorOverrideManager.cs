namespace AFV2
{
    using System.Collections.Generic;
    using AF;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimatorManager))]
    public class AnimatorOverrideManager : MonoBehaviour
    {
        AnimatorManager animatorManager => GetComponent<AnimatorManager>();
        Animator animator => GetComponent<Animator>();
        AnimatorOverrideController animatorOverrideController;

        [SerializeField] AnimationEventDispatcher animationEventDispatcher;

        private void Awake()
        {
            SetupAnimatorOverrides();
        }


        void SetupAnimatorOverrides()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        public void UpdateClips(Dictionary<string, ActionClip> animationUpdates)
        {
            if (animatorOverrideController == null)
            {
                SetupAnimatorOverrides();
            }

            AnimationClipOverrides clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
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

                foreach (var actionClipEvents in actionClip.Events)
                {
                    string eventName = $"{animationName}_{actionClipEvents.triggerTime}";

                    // Register event in dispatcher
                    animationEventDispatcher.RegisterEvent(eventName, actionClipEvents.OnEvent);

                    // Create and assign AnimationEvent
                    UnityEngine.AnimationEvent animationEvent = new();
                    animationEvent.functionName = "TriggerEvent";
                    animationEvent.stringParameter = eventName;
                    animationEvent.time = actionClipEvents.triggerTime;

                    clipOverrides[animationName].AddEvent(animationEvent);
                }
            }

            animatorOverrideController.ApplyOverrides(clipOverrides);
        }

    }
}
