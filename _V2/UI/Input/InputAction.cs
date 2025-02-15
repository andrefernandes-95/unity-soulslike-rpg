namespace AFV2
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputAction : MonoBehaviour
    {
        public Sprite ps4Icon;
        public Sprite xboxIcon;

        public string GetCurrentKeyBinding(PlayerInput playerInput)
        {
            if (playerInput.actions[name] == null)
            {
                Debug.LogError($"{name} not found in playerInput.actions");
                return "";
            }

            var action = playerInput.actions[name];
            return action.bindings[0].effectivePath.Replace("<Keyboard>/", "").ToUpper() + ")";
        }
    }
}
