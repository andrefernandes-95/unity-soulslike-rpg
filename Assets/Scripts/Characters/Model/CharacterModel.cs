namespace AFV2
{
    using UnityEngine;

    public class CharacterModel : MonoBehaviour
    {
        [Header("Transform References")]
        public Transform LeftHand;
        public Transform RightHand;
        public Transform LeftFoot;
        public Transform RightFoot;
        public Transform Torso;


        [Header("Body Parts")]
        [SerializeField] SkinnedMeshRenderer[] defaultEyebrow;
        [SerializeField] SkinnedMeshRenderer[] defaultEyes;
        [SerializeField] SkinnedMeshRenderer[] defaultMouth;
        [SerializeField] SkinnedMeshRenderer[] defaultFacialHair;
        [SerializeField] SkinnedMeshRenderer[] defaultHair;

        [SerializeField] SkinnedMeshRenderer[] defaultHead;
        [SerializeField] SkinnedMeshRenderer[] defaultTorso;
        [SerializeField] SkinnedMeshRenderer[] defaultHands;
        [SerializeField] SkinnedMeshRenderer[] defaultLegs;

        [Header("Bone Names")]
        public string leftHandBoneName = "Left_Hand";
        public string rightHandBoneName = "Right_Hand";
        public string leftFootBoneName = "Left_Foot";
        public string rightFootBoneName = "Right_Foot";
        public string torsoBoneName = "Hips";


        private void OnTransformChildrenChanged()
        {
            Debug.Log("Child objects changed! Reassigning bones...");
            AssignBoneReferences();
        }

        public void AssignBoneReferences()
        {
            LeftHand = FindBone(leftHandBoneName);
            RightHand = FindBone(rightHandBoneName);
            LeftFoot = FindBone(leftFootBoneName);
            RightFoot = FindBone(rightFootBoneName);
            Torso = FindBone(torsoBoneName);

            Debug.Log("Bone reassignment complete.");
        }

        private Transform FindBone(string boneName)
        {
            Transform foundBone = transform.Find(boneName);
            if (foundBone == null)
            {
                foreach (Transform child in GetComponentsInChildren<Transform>())
                {
                    if (child.name.Contains(boneName))
                    {
                        return child;
                    }
                }
            }
            return foundBone;
        }

        public void EnableCharacterModel()
        {
            gameObject.SetActive(true);
        }

        public void DisableCharacterModel()
        {
            GetComponentInParent<Animator>().avatar = null;
            gameObject.SetActive(false);
        }

        void EnableOrDisableCollection(SkinnedMeshRenderer[] collection, bool value)
        {
            foreach (var piece in collection)
                piece.enabled = value;
        }

        #region Unity Events - Used to restore default appearance when changing equipment
        public void ShowHair() => EnableOrDisableCollection(defaultHair, true);
        public void HideHair() => EnableOrDisableCollection(defaultHair, false);

        public void ShowFacialHair() => EnableOrDisableCollection(defaultFacialHair, true);
        public void HideFacialHair() => EnableOrDisableCollection(defaultFacialHair, false);

        public void ShowEyes() => EnableOrDisableCollection(defaultEyes, true);
        public void HideEyes() => EnableOrDisableCollection(defaultEyes, false);

        public void ShowEyebrows() => EnableOrDisableCollection(defaultEyebrow, true);
        public void HideEyebrows() => EnableOrDisableCollection(defaultEyebrow, false);

        public void ShowMouth() => EnableOrDisableCollection(defaultMouth, true);
        public void HideMouth() => EnableOrDisableCollection(defaultMouth, false);

        public void ShowHead() => EnableOrDisableCollection(defaultHead, true);
        public void HideHead() => EnableOrDisableCollection(defaultHead, false);

        public void ShowTorso() => EnableOrDisableCollection(defaultTorso, true);
        public void HideTorso() => EnableOrDisableCollection(defaultTorso, false);

        public void ShowHands() => EnableOrDisableCollection(defaultHands, true);
        public void HideHands() => EnableOrDisableCollection(defaultHands, false);

        public void ShowLegs() => EnableOrDisableCollection(defaultLegs, true);
        public void HideLegs() => EnableOrDisableCollection(defaultLegs, false);
        #endregion
    }
}
