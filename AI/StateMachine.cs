namespace AF
{
    using UnityEngine;

    public class StateManager : MonoBehaviour
    {
        public State currentState;

        [Header("State References")]
        public IdleState idleState;
        public ChaseState chaseState;
        public AmbushState ambushState;
        public CombatState combatState;

        private State defaultState;
        private State scheduledState; // Holds the next state transition

        private void Awake()
        {
            this.defaultState = currentState;
        }

        private void Start()
        {
            currentState?.OnStateEnter(this);
        }

        void Update()
        {
            if (currentState == null)
            {
                return;
            }

            // Execute state logic and queue next state
            State nextState = currentState.Tick(this);

            // Don't transition immediately—queue it first
            if (nextState != null && nextState != currentState)
            {
                scheduledState = nextState;
            }

            // Apply state change at the end of the frame
            if (scheduledState != null)
            {
                ApplyStateChange();
            }
        }

        private void ApplyStateChange()
        {
            if (scheduledState == null || scheduledState == currentState)
            {
                return;
            }

            State oldState = currentState;
            currentState = scheduledState;
            scheduledState = null; // Clear after transition

            oldState?.OnStateExit(this);
            currentState.OnStateEnter(this);
        }

        public void ScheduleState(State nextState)
        {
            if (nextState == null || nextState == currentState)
            {
                return;
            }

            scheduledState = nextState; // Queue the state for change
        }

        public bool IsInAmbushState() => currentState == ambushState;

        public void ResetDefaultState()
        {
            if (defaultState != null)
            {
                ScheduleState(defaultState);
            }
        }
    }
}
