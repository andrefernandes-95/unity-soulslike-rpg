namespace AFV2
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AssetCredits))]
    public class AssetCreditEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            AssetCredits assetCredits = (AssetCredits)target;
            if (GUILayout.Button("Get Asset", EditorStyles.toolbarButton))
            {
                Application.OpenURL(assetCredits.assetURL);
            }

            // Apply changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(assetCredits);
            }
        }
    }
}
