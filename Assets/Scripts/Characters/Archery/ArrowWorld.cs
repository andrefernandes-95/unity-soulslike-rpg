namespace AFV2
{
    using UnityEngine;

    public class ArrowWorld : MonoBehaviour
    {
        public Arrow arrow;

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
            GameObject projectile = Instantiate(this.gameObject, position, rotation);

            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.linearVelocity = rb.transform.forward * arrow.shootForce;

        }
    }
}
