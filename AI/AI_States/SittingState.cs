namespace AF
{
    using UnityEngine;
    using UnityEngine.Events;

    public class SittingState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Optional - Animation Clip")]
        public string animationClipToOverride = "Sitting Idle at Table";
        public AnimationClip overrideClip;
        readonly string sittingAnimationHash = "Sitting";

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        /// <summary>
        /// Character must start in sitting state for this to work
        /// </summary>
        Vector3 sittingPosition;
        Quaternion sittingRotation;

        private void Awake()
        {
            sittingPosition = characterManager.transform.position;
            sittingRotation = characterManager.transform.rotation;

            if (overrideClip != null)
            {
                characterManager.UpdateAnimatorOverrideControllerClips(
                    new() { { animationClipToOverride, overrideClip } }
                    );
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            characterManager.agent.ResetPath();
            characterManager.StopAgentSpeed();

            if (!characterManager.transform.position.Equals(sittingPosition))
            {
                characterManager.characterTeleportManager.Teleport(sittingPosition, sittingRotation);
            }

            characterManager.PlayBusyAnimationWithRootMotion(sittingAnimationHash);
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }
        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            return this;
        }
    }
}
