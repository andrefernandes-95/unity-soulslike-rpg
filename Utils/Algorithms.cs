namespace AF
{
    using System;
    public static class Algorithms
    {
        public static bool IsPrime(int numberToTest)
        {
            if (numberToTest < 2)
            {
                return false;
            }

            for (int i = 2; i <= Math.Sqrt(numberToTest); i++)
            {
                if (numberToTest % i == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
