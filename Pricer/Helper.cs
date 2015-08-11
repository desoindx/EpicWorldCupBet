using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Pricer
{
    public static class Helper
    {
        private static Random rng = new Random();

        public static int NextPoisson(double lambda)
        {
            double p = 1.0;
            var l = Math.Exp(-lambda);
            int k = 0;
            do
            {
                k++;
                p *= rng.NextDouble();
            }
            while (p > l);
            return k - 1;
        }

        public static double NextDouble()
        {
            return rng.NextDouble();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<int> Permute(this IList<int> list, bool canBeFromSameGroup)
        {
            var result = new List<int>(list);
            var n = result.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = result[k];

                if (!canBeFromSameGroup && k == n)
                {
                    n++;
                    continue;
                }

                result[k] = result[n];
                result[n] = value;
            }

            return result;
        }

        public static List<int> Permutation(int n, bool canStayInPlace)
        {
            var permutation = new List<int>(n);
            for (int i = 0; i < n; i++)
            {
                int k = rng.Next(n - 1);
                if (canStayInPlace && i == k)
                {
                    i--;
                    continue;
                }
                permutation.Add(k);
            }

            return permutation;
        }
    }
}
