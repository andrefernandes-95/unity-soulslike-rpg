namespace AFV2
{
    using UnityEngine;
    using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [ExecuteInEditMode] // Runs in the editor without entering Play mode
    public class UniqueID : MonoBehaviour
    {
        [SerializeField] private string _id = "";

        public string ID => _id; // Read-only access

        private void Reset()
        {
            GenerateNewID();
        }

        private void Awake()
        {
            if (string.IsNullOrEmpty(_id))
            {
                GenerateNewID();
            }
        }

        private void GenerateNewID()
        {
            _id = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this); // Marks the object as changed in the editor
#endif
        }
    }

}
