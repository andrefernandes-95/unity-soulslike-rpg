namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class AnimatorManager : MonoBehaviour
    {
        Animator animator => GetComponent<Animator>();

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
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) || animator.IsInTransition(0))
            {
                await Task.Yield(); // Wait for next frame
            }

            // Now wait until the animation reaches the end
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < end)
            {
                await Task.Yield(); // Wait for next frame
            }
        }

    }
}
