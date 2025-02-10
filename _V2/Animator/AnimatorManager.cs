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
            animator.CrossFade(nextAnimation, blendTime);
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


    }
}
