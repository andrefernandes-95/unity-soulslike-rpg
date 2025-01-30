namespace AF
{
    using UnityEngine;
    using UnityEngine.Events;

    public class AmbushState : State
    {
        public string ambushIdle = "Ambush";
        public string exitAmbush = "Exiting Ambush";

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;
        public UnityEvent onAmbushBegin;
        public UnityEvent onAmbushFinish;

        [Header("Transitions")]
        public ChaseState chaseState;

        public bool isInAmbushState = false;
        public bool ambushHasFinished = false;

        [Header("SFX")]
        public AudioClip wakeUpSfx;


        public void WakeUpFromAmbush()
        {
            if (!isInAmbushState)
            {
                return;
            }

            if (wakeUpSfx != null)
            {
                characterManager.combatAudioSource.PlayOneShot(wakeUpSfx);
            }

            onAmbushBegin?.Invoke();

            characterManager.animator.CrossFade(exitAmbush, 0.1f);
        }

        public void ExitAmbushState()
        {
            ambushHasFinished = true;
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            isInAmbushState = true;
            ambushHasFinished = false;

            if (characterManager.agent.isOnNavMesh)
            {
                characterManager.agent.ResetPath();
            }

            characterManager.StopAgentSpeed();

            characterManager.animator.Play(ambushIdle);
        }

        public override void OnStateExit(StateManager stateManager)
        {
            isInAmbushState = false;
            onStateExit?.Invoke();
        }

        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            if (ambushHasFinished)
            {
                onAmbushFinish?.Invoke();

                return chaseState;
            }

            return this;
        }
    }
}
