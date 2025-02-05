namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class Grindwheel : ApplyWeaponBuff
    {
        public new virtual void Apply()
        {
            base.Apply();

            AnimateGrindingWheel();
        }

        public void AnimateGrindingWheel()
        {
            StartCoroutine(AnimateGrindingWheel_Coroutine());
        }

        IEnumerator AnimateGrindingWheel_Coroutine()
        {
            // Define the rotation increment
            float rotationSpeed = 90f; // Adjust as needed

            // Define the total rotation angle
            float totalRotation = 0f;

            // Loop until the total rotation reaches 180 degrees
            while (totalRotation < 180f)
            {
                // Calculate the rotation for this frame
                float rotationThisFrame = rotationSpeed * Time.deltaTime;

                // Rotate the grinding wheel
                grindingWheel.transform.Rotate(0, 0, rotationThisFrame);

                // Update the total rotation
                totalRotation += rotationThisFrame;

                // Wait for the next frame
                yield return null;
            }
        }
    }
}