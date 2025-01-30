namespace AF
{
    using AF.Events;
    using UnityEngine;

    [RequireComponent(typeof(GenericTrigger))]
    [RequireComponent(typeof(Moment))]
    public class Conversation : MonoBehaviour
    {

        public CharacterManager npc;

        Moment conversation => GetComponent<Moment>();

        private State stateCharacterWasInBeforeConversationStarted;

        private void Awake()
        {
            conversation.onMoment_Start.AddListener(OnStartConversation);
            conversation.onMoment_End.AddListener(OnEndConversation);
        }

        void OnStartConversation()
        {
            // End Greeting If Present
            npc.greetingMessageController.HideGreeting();

            npc.FacePlayer();

            // Stop Movement
            npc.StopAgentSpeed();

            npc.agent.isStopped = true;

            StopStateMachine();
        }

        void OnEndConversation()
        {
            npc.agent.isStopped = false;

            npc.FaceInitialRotation();

            ResumeStateMachine();
        }

        void StopStateMachine()
        {
            stateCharacterWasInBeforeConversationStarted = npc.stateManager.currentState;
            npc.stateManager.ScheduleState(npc.stateManager.idleState);
            npc.stateManager.enabled = false;
        }

        void ResumeStateMachine()
        {
            if (stateCharacterWasInBeforeConversationStarted != null)
            {
                npc.stateManager.ScheduleState(stateCharacterWasInBeforeConversationStarted);
                stateCharacterWasInBeforeConversationStarted = null;
            }

            npc.stateManager.enabled = true;
        }
    }
}
