namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EV_UpdateAnimatorClip : EventBase
    {
        public string nameOfAnimationClipToOverride;
        public AnimationClip animationClip;
        public CharacterManager characterManager;

        public override IEnumerator Dispatch()
        {
            Dictionary<string, AnimationClip> clips = new() {
                { nameOfAnimationClipToOverride, animationClip },
            };

            characterManager.UpdateAnimatorOverrideControllerClips(clips);

            yield return null;
        }
    }

}
