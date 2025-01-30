namespace AF.Dialogue
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    public class GreetingMessageController : MonoBehaviour
    {
        [Header("Greeting Settings")]
        public CharacterGreeting[] characterGreetings;
        public float greetingCooldown = 15f;
        public float minGreetingDelay = 0f;
        public float maxGreetingDelay = 0f;
        public bool showWhenAggressive = false;

        [Header("References")]
        public CharacterManager characterManager;
        public GreetingMessageUI greetingMessageUI;

        [Header("Events")]
        public UnityEvent onGreetingStart;
        public UnityEvent onGreetingEnd;

        private bool hasDisplayed;
        private Coroutine cooldownCoroutine;

        private void Awake()
        {
            characterManager?.targetManager?.onAgressiveTowardsPlayer_Event.AddListener(HandleAggression);
        }

        void HandleAggression()
        {
            if (showWhenAggressive)
            {
                ShowGreeting();
            }
            else
            {
                HideGreeting();
            }
        }

        public void ShowGreeting()
        {
            if (hasDisplayed || characterGreetings == null || characterGreetings.Length == 0)
                return;

            var greeting = characterGreetings.FirstOrDefault(g => g?.isActiveAndEnabled == true);
            if (greeting != null)
                StartCoroutine(GreetingRoutine(greeting));
        }

        private IEnumerator GreetingRoutine(CharacterGreeting greeting)
        {
            hasDisplayed = true;
            yield return new WaitForSeconds(Random.Range(minGreetingDelay, maxGreetingDelay));

            onGreetingStart?.Invoke();
            greetingMessageUI?.Display(greeting.greeting);

            yield return new WaitForSeconds(greeting.duration);
            HideGreeting();

            cooldownCoroutine = StartCoroutine(GreetingCooldown());
        }

        public void HideGreeting()
        {
            if (!hasDisplayed) return;

            hasDisplayed = false;
            onGreetingEnd?.Invoke();
            greetingMessageUI?.Hide();
        }

        private IEnumerator GreetingCooldown()
        {
            yield return new WaitForSeconds(greetingCooldown);
            hasDisplayed = false;
        }

        public void ResetGreetingState()
        {
            if (cooldownCoroutine != null)
                StopCoroutine(cooldownCoroutine);

            hasDisplayed = false;
        }
    }
}
