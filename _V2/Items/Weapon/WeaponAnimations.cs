namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public class WeaponAnimations : MonoBehaviour
    {
        [SerializeField] Transform commonActionClipsContainer;
        [SerializeField] Transform oneHandActionClipsContainer;
        [SerializeField] Transform twoHandActionClipsContainer;
        Dictionary<string, ActionClip> oneHandActionClips = new();
        Dictionary<string, ActionClip> twoHandActionClips = new();

        [Header("Available Animations")]
        [SerializeField] protected List<string> rightLightAttacks = new() { "Right Attack A", "Right Attack B", "Right Attack C" };
        public List<string> RightLightAttacks => rightLightAttacks.ToList();
        [SerializeField] protected List<string> leftLightAttacks = new() { "Left Attack A", "Left Attack B", "Left Attack C" };
        public List<string> LeftLightAttacks => leftLightAttacks.ToList();
        [SerializeField] protected List<string> heavyAttacks = new() { "Heavy Attack A", "Heavy Attack B", "Heavy Attack C" };
        public List<string> HeavyAttacks => heavyAttacks.ToList();
        [SerializeField] protected List<string> rightAirAttacks = new() { "Right Air Attack A" };
        public List<string> RightAirAttacks => rightAirAttacks.ToList();
        [SerializeField] protected List<string> leftAirAttacks = new() { "Left Air Attack A" };
        public List<string> LeftAirAttacks => leftAirAttacks.ToList();

        CharacterApi characterApi;

        private void OnEnable()
        {
            SetupCharacterApi();
            CollectActionClips();
        }

        void SetupCharacterApi()
        {
            if (characterApi == null)
                characterApi = GetComponentInParent<CharacterApi>();
        }

        void CollectActionClips()
        {
            foreach (ActionClip commonClip in commonActionClipsContainer.GetComponentsInChildren<ActionClip>())
            {
                oneHandActionClips.Add(commonClip.name, commonClip);
                twoHandActionClips.Add(commonClip.name, commonClip);
            }

            foreach (ActionClip oneHandActionClip in oneHandActionClipsContainer.GetComponentsInChildren<ActionClip>())
            {
                oneHandActionClips.Add(oneHandActionClip.name, oneHandActionClip);
            }

            foreach (ActionClip twoHandActionClip in twoHandActionClipsContainer.GetComponentsInChildren<ActionClip>())
            {
                twoHandActionClips.Add(twoHandActionClip.name, twoHandActionClip);
            }
        }

        public async void ApplyAnimations()
        {
            while (characterApi.animatorManager.IsInTransition())
                await Task.Yield();

            characterApi.animatorManager.animatorOverrideManager.UpdateClips(
                characterApi.characterEquipment.characterWeapons.IsTwoHanding ? twoHandActionClips : oneHandActionClips
            );
        }


    }
}
