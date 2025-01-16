namespace AF
{
    using UnityEngine;

    [RequireComponent(typeof(EV_SetTransformRotation))]
    public class DestroyableBox : MonoBehaviour
    {
        public GameObject destroyedBoxGraphic;
        public MeshRenderer intactBoxGraphic;
        EV_SetTransformRotation eV_SetTransformRotation => GetComponent<EV_SetTransformRotation>();

        public void DestroyBox()
        {
            intactBoxGraphic.enabled = false;
            destroyedBoxGraphic.gameObject.SetActive(true);
            eV_SetTransformRotation.Execute();

            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
