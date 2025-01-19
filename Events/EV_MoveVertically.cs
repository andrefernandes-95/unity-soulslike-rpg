namespace AF.Events
{
    using System.Collections;
    using UnityEngine;

    public class EV_MoveVertically : EventBase
    {
        public Transform objectToMove;
        public float yTargetPosition;
        public float movementSpeed = 2f;

        [Header("Settings")]
        float elapsedTime = 0f;
        public float maxTimeToTryReachingThePlace = 5f;

        public override IEnumerator Dispatch()
        {
            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;

                objectToMove.Translate(new Vector3(0, 0.1f * Time.deltaTime * movementSpeed, 0));

                return objectToMove.transform.position.y >= yTargetPosition || elapsedTime >= maxTimeToTryReachingThePlace;
            });
        }
    }
}
