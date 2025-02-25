namespace AFV2
{
    using UnityEngine;

    /// <summary>
    /// Responsible for managing the current 3D Mesh used by the player (i.e. Switch between different characters - knight, mage, skeleton, etc.)
    /// </summary>
    public class CharacterModelManager : MonoBehaviour
    {
        CharacterModel[] characterModels => GetComponentsInChildren<CharacterModel>();
        [SerializeField] CharacterModel activeCharacterModel;

        public Transform RightFoot => activeCharacterModel?.RightFoot;
        public Transform LeftFoot => activeCharacterModel?.LeftFoot;
        public Transform RightHand => activeCharacterModel?.RightHand;
        public Transform LeftHand => activeCharacterModel?.LeftHand;

        void Start()
        {
            if (activeCharacterModel != null)
            {
                SetCharacterModel(activeCharacterModel);
            }
        }

        public void SetCharacterModel(CharacterModel characterModel)
        {
            foreach (var model in characterModels)
            {
                model.DisableCharacterModel();
            }

            activeCharacterModel = characterModel;
            activeCharacterModel.EnableCharacterModel();
        }
    }
}
