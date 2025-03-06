namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class Consumable : Item
    {
        [Header("Options")]
        public bool isReplenishable = false;

        [Header("Events")]
        public UnityEvent onConsumed;
    }
}
