namespace AF
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Char Animation Overrides Template", menuName = "Data/New Char Animation Overrides Template", order = 0)]
    public class CharacterAnimationOverridesTemplate : ScriptableObject
    {
        [Header("Animation Clip Overrides")]
        public SerializedDictionary<AnimationOverrideKey, AnimationClip> clipOverrides;
    }
}
