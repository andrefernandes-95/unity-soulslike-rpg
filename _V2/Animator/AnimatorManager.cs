namespace AFV2
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Animator))]
    public class AnimatorManager : MonoBehaviour
    {
        public CharacterApi characterApi;

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

        public async Task RunActionClip(ActionClip actionClip, float end = .9f)
        {
            // Ensure the animator exists
            if (animator == null)
                return;

            // Wait until the new animation is fully playing
            while (!IsAnimationPlaying(actionClip.name))
            {
                await Task.Yield(); // Wait for next frame
            }

            Dictionary<float, UnityEvent> events = new();
            foreach (var actionClipEvent in actionClip.Events)
            {
                events.Add(actionClipEvent.triggerTime, actionClipEvent.OnEvent);
            }

            HashSet<float> triggeredEvents = new(); // Keep track of triggered events

            // Now wait until the animation reaches the end
            while (!HasAnimationEnded(end))
            {
                float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

                foreach (var kvp in events)
                {
                    float eventTime = kvp.Key;
                    UnityEvent unityEvent = kvp.Value;

                    if (currentTime >= eventTime && !triggeredEvents.Contains(eventTime))
                    {
                        unityEvent?.Invoke();
                        triggeredEvents.Add(eventTime);
                    }
                }

                await Task.Yield(); // Wait for next frame
            }
        }

        public bool IsAnimationPlaying(string animationName)
        {
            if (animator.IsInTransition(0))
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
