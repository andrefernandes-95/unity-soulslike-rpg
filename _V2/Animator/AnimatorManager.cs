namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimatorOverrideManager))]
    public class AnimatorManager : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        [HideInInspector] public AnimatorOverrideManager animatorOverrideManager => GetComponent<AnimatorOverrideManager>();

        Animator animator => GetComponent<Animator>();

        private void OnAnimatorMove()
        {
            if (!animator.applyRootMotion) return;

            Quaternion rootMotionRotation = animator.deltaRotation;
            characterApi.transform.rotation *= rootMotionRotation;

            // Extract root motion position and rotation from the Animator
            Vector3 rootMotionPosition = animator.deltaPosition + new Vector3(0.0f, -9, 0.0f) * Time.deltaTime;

            if (characterApi.characterController.enabled)
                characterApi.characterController.Move(rootMotionPosition);
        }

        public void BlendTo(string nextAnimation, float blendTime = 0.2f)
        {
            if (animator.fireEvents == false)
                Invoke(nameof(RefireEvents), 0f);

            animator.CrossFade(nextAnimation, blendTime);
        }

        void RefireEvents()
        {
            animator.fireEvents = true;
            Debug.Log($"Refirede events at {Time.time}");
        }

        public async Task WaitForAnimationToFinish(string animationName, float end = .9f)
        {
            // Ensure the animator exists
            if (animator == null)
                return;

            // Wait until the new animation is fully playing
            while (!IsAnimationPlaying(animationName))
            {
                await Task.Yield(); // Wait for next frame
            }

            // Now wait until the animation reaches the end
            while (!HasAnimationEnded(end))
            {
                await Task.Yield(); // Wait for next frame
            }
        }

        public bool IsInTransition() => animator.IsInTransition(0);

        public bool IsAnimationPlaying(string animationName)
        {
            if (IsInTransition())
            {
                return false;
            }

            return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }

        public bool HasAnimationEnded(float end)
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= end;
        }

        public void EnableRootMotion() => animator.applyRootMotion = true;
        public void DisableRootMotion() => animator.applyRootMotion = false;

        // Retrieves the progress of the currently playing animation
        public float GetAnimationProgress(string animationName)
        {
            if (animator == null) return 1.0f; // Return 100% if no animator is found

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // If the current animation matches the one we are tracking, return its normalized time
            if (stateInfo.IsName(animationName))
            {
                return stateInfo.normalizedTime % 1.0f; // Normalize it to stay within 0 - 1
            }

            return 1.0f; // Assume complete if it's not found
        }
    }
}
