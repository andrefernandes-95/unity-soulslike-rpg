namespace AFV2
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(CharacterModel))]
    public class CharacterModelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup Character"))
            {
                ((CharacterModel)target).AssignBoneReferences();
            }

            GUILayout.Space(10);

            GUILayout.Box("Click on Setup Character when changing characters to search for the bones and assign them automatically");

            DrawDefaultInspector();

        }
    }
#endif

}