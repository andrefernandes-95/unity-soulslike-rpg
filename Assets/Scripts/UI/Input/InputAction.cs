namespace AFV2
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    [CreateAssetMenu(fileName = "Input Action", menuName = "AFV2 / Data / New Input Action", order = 0)]
    public class InputAction : ScriptableObject
    {
        [Tooltip("Must match the action name in StarterAssetsInput")]
        public string actionName;
        public Sprite ps4Icon;
        public Sprite xboxIcon;

        public string GetCurrentKeyBinding(PlayerInput playerInput)
        {
            if (playerInput.actions[actionName] == null)
            {
                Debug.LogError($"{actionName} not found in playerInput.actions");
                return "";
            }

            var action = playerInput.actions[actionName];
            return action.bindings[0].effectivePath.Replace("<Keyboard>/", "").ToUpper();
        }
    }
}
