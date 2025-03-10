namespace AFV2
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    public class OnTriggerEvents : MonoBehaviour
    {
        [Header("Object detection")]
        public CharacterApi target;

        [Header("Tags")]
        public string[] tagsToDetect;

        [Header("Settings")]
        public float intervalBetweenInvokations = 1f;

        bool m_canTriggerOnStay = true;
        public bool CanTriggerOnStay
        {
            get
            {
                return m_canTriggerOnStay;
            }

            set
            {
                if (value == false)
                {
                    Invoke(nameof(ResetCanTrigger), intervalBetweenInvokations);
                }

                m_canTriggerOnStay = value;
            }
        }

        [Header("Events")]
        public UnityEvent onTriggerEnterEvent;
        public UnityEvent onTriggerStayEvent;
        public UnityEvent onTriggerExitEvent;

        bool isActive = true;

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public void SetIsActive(bool value)
        {
            isActive = value;
        }

        bool ShouldTrigger(Collider collider)
        {
            if (!isActive)
            {
                return false;
            }

            if (collider.TryGetComponent<CharacterApi>(out var collidingCharacter) && collidingCharacter == target)
            {
                return true;
            }

            if (tagsToDetect.Contains(collider.tag))
            {
                return true;
            }

            return false;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (ShouldTrigger(collider))
            {
                onTriggerEnterEvent?.Invoke();
            }
        }

        public void OnTriggerStay(Collider collider)
        {
            if (!CanTriggerOnStay)
            {
                return;
            }

            if (ShouldTrigger(collider))
            {
                onTriggerStayEvent?.Invoke();

                CanTriggerOnStay = false;
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (ShouldTrigger(collider))
            {
                onTriggerExitEvent?.Invoke();
            }
        }

        void ResetCanTrigger()
        {
            CanTriggerOnStay = true;
        }
    }
}
