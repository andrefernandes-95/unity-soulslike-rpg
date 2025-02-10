namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class StateMachine : MonoBehaviour
    {
        public State currentState;

        [Header("State References")]
        private State defaultState;
        private State scheduledState; // Holds the next state transition

        private bool isTransitioning = false; // Flag to track async transitions

        private void Awake()
        {
            this.defaultState = currentState;
        }

        private void Start()
        {
            currentState?.OnStateEnter();
        }

        void Update()
        {
            if (currentState == null || isTransitioning)
            {
                return;
            }

            // Execute state logic and queue next state
            State nextState = currentState.Tick();

            // Don't transition immediately—queue it first
            if (nextState != null && nextState != currentState)
            {
                ScheduleState(nextState);
            }

            // Apply state change at the end of the frame
            if (scheduledState != null)
            {
                ApplyStateChange();
            }
        }

        private async void ApplyStateChange()
        {
            if (scheduledState == null || scheduledState == currentState)
            {
                return;
            }

            isTransitioning = true; // Mark transition as in progress

            State oldState = currentState;
            currentState = scheduledState;
            scheduledState = null; // Clear after transition

            if (currentState.priority <= oldState.priority)
            {
                Task onExitTask = oldState.OnStateExit();
                if (onExitTask != null)
                {
                    await onExitTask; // Properly wait for the async method to finish
                }
            }

            currentState.OnStateEnter();
            isTransitioning = false; // Mark transition as complete
        }

        public void ScheduleState(State nextState)
        {
            if (nextState == null || nextState == currentState)
            {
                return;
            }

            scheduledState = nextState; // Queue the state for change
        }

        public void ResetDefaultState()
        {
            if (defaultState != null)
            {
                ScheduleState(defaultState);
            }
        }
    }
}
