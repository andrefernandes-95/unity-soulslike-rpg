namespace AF
{
    using UnityEngine;

    public abstract class State : MonoBehaviour
    {
        public abstract void OnStateEnter(StateManager stateManager);
        public abstract void OnStateExit(StateManager stateManager);
        public abstract State Tick(StateManager stateManager);
    }

}