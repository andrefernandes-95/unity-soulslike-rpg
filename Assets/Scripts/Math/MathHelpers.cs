namespace AFV2
{
    using UnityEngine;
    public static class MathHelpers
    {
        static float Sigmoid(float normalizedValue)
        {
            float value = Mathf.Exp(-10f * (normalizedValue - 0.5f));
            return 1f / (1f + value);
        }

        /// <summary>
        ///  More chance when normalized value is high, less when normalized value is low
        /// </summary>
        public static float PositiveSigmoid(float normalizedValue) => Sigmoid(normalizedValue);

        /// <summary>
        ///  Less chance when normalized value is high, more when normalized value is low
        /// </summary>
        public static float NegativeSigmoid(float normalizedValue) => 1f - Sigmoid(normalizedValue);
    }
}
