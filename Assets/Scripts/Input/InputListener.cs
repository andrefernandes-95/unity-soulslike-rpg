namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;

    public class InputListener : MonoBehaviour
    {
        Vector2 move;
        public Vector2 Move
        {
            get { return move; }
        }

        public UnityEvent onNavigate;

        Vector2 look;
        public Vector2 Look
        {
            get { return look; }
        }

        bool cursorInputForLook = true;
        Vector2 scaleVector = new(1, 1);

        bool sprint;
        public bool Sprint
        {
            get { return sprint; }
        }

        bool dodge;
        public bool Dodge
        {
            get { return dodge; }
        }

        bool jump;
        public bool Jump
        {
            get { return jump; }
        }

        [Header("Events")]
        public UnityEvent onHeavyAttack;
        public UnityEvent onRightAttack;
        public UnityEvent onLeftAttack;
        public UnityEvent onChangeCombatStance;
        public UnityEvent onMenu;
        public UnityEvent onInteract;
        public UnityEvent onQuickSave;
        public UnityEvent onQuickLoad;

        public UnityEvent onSwitchRightWeapon;
        public UnityEvent onSwitchLeftWeapon;
        public UnityEvent onSwitchSpell;
        public UnityEvent onSwitchConsumable;

        public UnityAction<float> onZoomIn;
        public UnityAction<float> onZoomOut;

        [Header("Components")]
        [SerializeField] GameSettings gameSettings;

        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                look = value.Get<Vector2>();
            }

            if (scaleVector.x != gameSettings.cameraSensitivity)
            {
                scaleVector = new(gameSettings.cameraSensitivity, gameSettings.cameraSensitivity);
            }

            look.Scale(scaleVector);
        }

        public void OnSprint(InputValue value)
        {
            sprint = value.isPressed;
        }

        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
            CancelInvoke(nameof(ResetJump)); // Cancel previous invoke if any
            Invoke(nameof(ResetJump), Time.deltaTime); // Schedule reset
        }

        void ResetJump() => jump = false;


        public void OnZoomIn(InputValue value)
        {
            if (value.isPressed)
            {
                onZoomIn?.Invoke(value.Get<float>());
            }
        }

        public void OnZoomOut(InputValue value)
        {
            if (value.isPressed)
            {
                onZoomOut?.Invoke(value.Get<float>());
            }
        }

        public void OnLightAttack(InputValue value)
        {
            if (value.isPressed)
                onRightAttack.Invoke();
        }

        public void OnHeavyAttack(InputValue value)
        {
            if (value.isPressed)
                onHeavyAttack.Invoke();
        }

        public void OnBlock(InputValue value)
        {
            if (value.isPressed)
                onLeftAttack.Invoke();
        }


        public void OnChangeCombatStance(InputValue value)
        {
            onChangeCombatStance?.Invoke();
        }

        public void OnMenu(InputValue value)
        {
            if (value.isPressed)
            {
                onMenu?.Invoke();
            }
        }

        public void OnSwitchSpell(InputValue value)
        {
            if (value.isPressed)
            {
                onNavigate?.Invoke();
                onSwitchSpell?.Invoke();
            }
        }

        public void OnSwitchConsumable(InputValue value)
        {
            if (value.isPressed)
            {
                onNavigate?.Invoke();
                onSwitchConsumable?.Invoke();
            }
        }

        public void OnSwitchWeapon(InputValue value)
        {
            if (value.isPressed)
            {
                onNavigate?.Invoke();
                onSwitchRightWeapon?.Invoke();
            }
        }

        public void OnSwitchShield(InputValue value)
        {
            if (value.isPressed)
            {
                onNavigate?.Invoke();
                onSwitchLeftWeapon?.Invoke();
            }
        }

        public void OnInteract(InputValue value)
        {
            if (value.isPressed)
            {
                onInteract?.Invoke();
            }
        }

        public void OnQuickSave(InputValue value)
        {
            if (value.isPressed)
            {
                onQuickSave?.Invoke();
            }
        }

        public void OnQuickLoad(InputValue value)
        {
            if (value.isPressed)
            {
                onQuickLoad?.Invoke();
            }
        }

        public void OnDodge(InputValue value)
        {
            if (value.isPressed)
            {
                dodge = true;
                CancelInvoke(nameof(ResetDodge)); // Cancel previous invoke if any
                Invoke(nameof(ResetDodge), Time.deltaTime); // Schedule reset
            }
        }
        void ResetDodge() => dodge = false;

    }
}
