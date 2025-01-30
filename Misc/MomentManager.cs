namespace AF
{
    using System.Collections.Generic;
    using AF.Events;
    using TigerForge;
    using UnityEngine;

    public class MomentManager : MonoBehaviour
    {
        [SerializeField]
        private List<Moment> ongoingMoments = new();

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MOMENT_START, AddMoment);
            EventManager.StartListening(EventMessages.ON_MOMENT_END, RemoveMoment);
        }

        private void AddMoment()
        {
            Moment moment = EventManager.GetData(EventMessages.ON_MOMENT_START) as Moment;

            if (ongoingMoments.Contains(moment))
            {
                return;
            }

            ongoingMoments.Add(moment);
        }

        private void RemoveMoment()
        {
            Moment moment = EventManager.GetData(EventMessages.ON_MOMENT_END) as Moment;

            if (ongoingMoments.Contains(moment))
            {
                ongoingMoments.Remove(moment);
            }
        }

        public bool HasMomentOngoing() => ongoingMoments.Count > 0;
    }
}
