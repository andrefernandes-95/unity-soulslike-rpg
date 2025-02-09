// CREDITS: https://github.com/adammyhre/Improved-Unity-Animation-Events

namespace AFV2
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ActionClip))]
    public class ActionClipEditor : Editor
    {
        private float previewTime;
        private bool isPreviewing;

        public override void OnInspectorGUI()
        {
            ActionClip actionClip = (ActionClip)target;
            if (actionClip == null)
            {
                EditorGUILayout.HelpBox("Error previewing action clip.", MessageType.Info);
                return;
            }

            GUILayout.Space(10);

            if (isPreviewing)
            {
                if (GUILayout.Button("Stop Preview"))
                {
                    EnforceTPose(Selection.activeGameObject);
                    isPreviewing = false;
                    AnimationMode.StopAnimationMode();
                }
                else
                {
                    PreviewAnimationClip(actionClip);
                }
            }
            else if (GUILayout.Button("Preview"))
            {
                isPreviewing = true;
                AnimationMode.StartAnimationMode();
            }

            GUILayout.Label($"Previewing at {previewTime:F2}s", EditorStyles.helpBox);

            serializedObject.ApplyModifiedProperties();
            DrawDefaultInspector();
        }

        private void PreviewAnimationClip(ActionClip actionClip)
        {
            if (actionClip.animationClip is AnimationClip clip)
            {
                previewTime = actionClip.PreviewTime * clip.length;
                Animator animator = GetValidAnimator(out _);

                if (animator != null)
                {
                    AnimationMode.SampleAnimationClip(animator.gameObject, clip, previewTime);
                }
            }
        }

        private Animator GetValidAnimator(out string errorMessage)
        {
            errorMessage = string.Empty;
            GameObject targetGameObject = Selection.activeGameObject;

            if (targetGameObject == null)
            {
                errorMessage = "Please select a GameObject with an Animator to preview.";
                return null;
            }

            Animator animator = targetGameObject
                .GetComponentInParent<CharacterApi>()
                ?.animatorManager
                ?.GetComponent<Animator>();

            if (animator == null)
            {
                errorMessage = "The hierarchy does not have a valid AnimatorController.";
                return null;
            }

            return animator;
        }

        [MenuItem("GameObject/Enforce T-Pose", false, 0)]
        private static void EnforceTPoseMenu()
        {
            EnforceTPose(Selection.activeGameObject);
        }

        private static void EnforceTPose(GameObject target)
        {
            if (!target || !target.TryGetComponent(out Animator animator) || !animator.avatar)
                return;

            SkeletonBone[] skeletonBones = animator.avatar.humanDescription.skeleton;

            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) continue;

                Transform boneTransform = animator.GetBoneTransform(bone);
                if (boneTransform == null) continue;

                SkeletonBone skeletonBone = skeletonBones.FirstOrDefault(sb => sb.name == boneTransform.name);
                if (skeletonBone.name == null) continue;

                if (bone == HumanBodyBones.Hips)
                    boneTransform.localPosition = skeletonBone.position;

                boneTransform.localRotation = skeletonBone.rotation;
            }
        }

    }
}
