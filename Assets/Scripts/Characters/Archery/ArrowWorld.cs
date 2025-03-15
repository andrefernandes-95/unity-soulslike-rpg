namespace AFV2
{
    using UnityEngine;

    public class ArrowWorld : MonoBehaviour
    {
        public Arrow arrow;

        [SerializeField] CharacterApi characterApi;

        Vector3 originalLocalPosition;
        Quaternion originalLocalRotation;
        Vector3 originalLocalScale;

        void Awake()
        {
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
            originalLocalScale = transform.localScale;
            transform.parent = characterApi.characterModel.LeftHand;
            transform.localPosition = originalLocalPosition;
            transform.localRotation = originalLocalRotation;
            transform.localScale = originalLocalScale;
        }

        public void OnArrowSwitched(Arrow equippedArrow)
        {
            if (equippedArrow == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(arrow == equippedArrow);
        }

        public void Shoot(Vector3 position, Quaternion rotation)
        {
            GameObject projectile = Instantiate(this.gameObject);
            projectile.transform.parent = null;
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;

            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.linearVelocity = rb.transform.forward * arrow.shootForce;

        }
    }
}
