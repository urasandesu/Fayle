using System;

namespace HowDoesPexWork
{
    class HiddenBug
    {
        public static void Puzzle(int[] v)
        {
            if (v != null &&
                v.Length > 0 &&
                v[0] == 1234567890)
                throw new Exception("hidden bug!");
        }
    }
}
