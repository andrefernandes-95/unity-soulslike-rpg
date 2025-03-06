namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ResetStatesOnStateEnter : StateMachineBehaviour
    {
        CharacterApi characterApi;
        List<IResetCharacterStatesOnStateEnterListener> listeners = new();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterApi == null)
                characterApi = animator.GetComponentInParent<CharacterApi>();

            if (listeners.Count <= 0)
                listeners = characterApi.GetComponentsInChildren<IResetCharacterStatesOnStateEnterListener>().ToList();

            foreach (var listener in listeners)
            {
                listener.ResetStates();
            }
        }
    }
}
