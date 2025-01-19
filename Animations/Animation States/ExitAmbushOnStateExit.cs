namespace AF
{
    using UnityEngine;

    public class ExitAmbushOnStateExit : StateMachineBehaviour
    {
        CharacterManager character;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            character?.stateManager?.ambushState?.ExitAmbushState();
        }
    }
}
