namespace AFV2
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class WeaponAnimations : MonoBehaviour
    {
        [SerializeField] Transform commonActionClipsContainer;
        [SerializeField] Transform oneHandActionClipsContainer;
        [SerializeField] Transform twoHandActionClipsContainer;
        Dictionary<string, ActionClip> oneHandActionClips = new();
        Dictionary<string, ActionClip> twoHandActionClips = new();

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
