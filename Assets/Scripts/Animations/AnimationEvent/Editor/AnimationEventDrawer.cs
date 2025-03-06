// Source: https://github.com/adammyhre/Improved-Unity-Animation-Events/blob/master/Assets/_Project/Scripts/AnimationEvents/Editor/AnimationEventDrawer.cs
namespace AFV2
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(AnimationEvent))]
    public class AnimationEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty stateNameProperty = property.FindPropertyRelative("triggerTime");

            SerializedProperty stateEventProperty = property.FindPropertyRelative("OnEvent");

            Rect stateNameRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect stateEventRect = new(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width,
                EditorGUI.GetPropertyHeight(stateEventProperty));

            EditorGUI.PropertyField(stateNameRect, stateNameProperty);
            EditorGUI.PropertyField(stateEventRect, stateEventProperty, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty stateEventProperty = property.FindPropertyRelative("OnEvent");
            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(stateEventProperty) + 4;
        }
    }
}
