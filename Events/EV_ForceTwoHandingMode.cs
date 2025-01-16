namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class EV_ForceTwoHandingMode : EventBase
    {
        public override IEnumerator Dispatch()
        {
            var twoHandingController = FindAnyObjectByType<TwoHandingController>(FindObjectsInactive.Include);
            twoHandingController?.equipmentDatabase.SetIsTwoHanding(true);

            yield return null;
        }
    }

}
