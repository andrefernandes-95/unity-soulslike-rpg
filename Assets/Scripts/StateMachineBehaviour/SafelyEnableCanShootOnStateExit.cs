namespace AFV2
{
    using UnityEngine;

    public class SafelyEnableCanShotOnStateExit : StateMachineBehaviour
    {
        CharacterApi characterApi;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterApi == null)
                characterApi = animator.GetComponentInParent<CharacterApi>();

            if (characterApi != null)
            {
                characterApi.characterArchery.onShotFinished?.Invoke();
            }
        }
    }
}
