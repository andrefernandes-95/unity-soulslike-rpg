namespace AFV2
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterStamina : MonoBehaviour, ISaveable
    {
        private float _stamina;
        [SerializeField] private int maxStamina = 200;
        public int MaxStamina => maxStamina;

        [HideInInspector] public UnityEvent onHealthChange, onStaminaChange, onManaChange;

        [Header("Stamina Regeneration")]
        [SerializeField] private float regenRate = 5f; // Stamina per second
        [SerializeField] private float regenDelay = 2f; // Delay before regen starts

        private Coroutine regenCoroutine;
        private float lastStaminaUseTime;

        public float Stamina
        {
            get => _stamina;
        }

        [Header("Action Costs")]
        [SerializeField] float jumpCost = 5f;
        [SerializeField] float sprintCost = .5f;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        void Start()
        {
            SetStamina(maxStamina);
        }

        public void SetStamina(float value)
        {
            _stamina = Mathf.Clamp(value, 0, maxStamina);
            onStaminaChange?.Invoke();

            if (regenCoroutine == null && Stamina < maxStamina)
            {
                regenCoroutine = StartCoroutine(RegenerateStamina());
            }
        }

        public bool HasEnoughStamina(float amount) => Stamina >= amount;

        public void DecreaseStamina(float value)
        {
            SetStamina(Stamina - value);
            lastStaminaUseTime = Time.time;
        }

        /// <summary>
        /// Normalize stamina between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float GetNormalizedStamina() => Stamina / maxStamina;

        #region Can Do Actions
        public bool CanSprint()
        {
            return Stamina > sprintCost;
        }

        #endregion

        #region Use Actions
        public void UseSprint()
        {
            DecreaseStamina(sprintCost * Time.deltaTime);
        }
        #endregion

        private IEnumerator RegenerateStamina()
        {
            yield return new WaitForEndOfFrame();

            while (Stamina < maxStamina)
            {
                if (Time.time - lastStaminaUseTime >= regenDelay)
                {
                    SetStamina(Stamina + regenRate * Time.deltaTime);
                }

                yield return null;
            }

            regenCoroutine = null;
        }

        public void SaveData(SaveWriter writer)
        {
            writer.Write(GetSavePath(), Stamina);
        }

        public void LoadData(SaveReader reader)
        {
            if (reader.TryRead(GetLoadPath(), "stamina", out float stamina))
            {
                SetStamina(stamina);
            }
        }

        string GetSavePath() => $"{characterApi.GetCharacterId()}.stamina";
        string GetLoadPath() => $"{characterApi.GetCharacterId()}";
    }
}
