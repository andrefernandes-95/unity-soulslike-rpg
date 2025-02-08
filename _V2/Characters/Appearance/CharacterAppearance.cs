namespace AFV2
{
    using UnityEngine;

    public class CharacterAppearance : MonoBehaviour
    {
        [Header("Naked Appearance")]
        [SerializeField] private SkinnedMeshRenderer defaultEyebrow;
        [SerializeField] private SkinnedMeshRenderer defaultEyes;
        [SerializeField] private SkinnedMeshRenderer defaultMouth;
        [SerializeField] private SkinnedMeshRenderer[] defaultFacialHair;
        [SerializeField] private SkinnedMeshRenderer[] defaultHair;

        [SerializeField] private SkinnedMeshRenderer defaultHead;
        [SerializeField] private SkinnedMeshRenderer defaultTorso;
        [SerializeField] private SkinnedMeshRenderer defaultLegs;

        public void ShowHair() => EnableOrDisableCollection(defaultHair, true);
        public void HideHair() => EnableOrDisableCollection(defaultHair, false);

        public void ShowFacialHair() => EnableOrDisableCollection(defaultFacialHair, true);
        public void HideFacialHair() => EnableOrDisableCollection(defaultFacialHair, false);

        public void ShowEyes() => defaultEyes.enabled = true;
        public void HideEyes() => defaultEyes.enabled = false;

        public void ShowEyebrows() => defaultEyebrow.enabled = true;
        public void HideEyebrows() => defaultEyebrow.enabled = false;

        public void ShowMouth() => defaultMouth.enabled = true;
        public void HideMouth() => defaultMouth.enabled = false;

        public void ShowHead() => defaultHead.enabled = true;
        public void HideHead() => defaultHead.enabled = false;

        public void ShowTorso() => defaultTorso.enabled = true;
        public void HideTorso() => defaultTorso.enabled = false;

        public void ShowLegs() => defaultLegs.enabled = true;
        public void HideLegs() => defaultLegs.enabled = false;

        void EnableOrDisableCollection(SkinnedMeshRenderer[] collection, bool value)
        {
            foreach (var piece in collection)
                piece.enabled = value;
        }
    }
}
