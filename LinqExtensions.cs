using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Extensions
    {
        public static float StdDev(this IEnumerable<float> values)
        {
            float ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = (float)Math.Sqrt(sum / count);
            }
            return ret;
        }

        public static float Median(this IEnumerable<float> source)
        {
            int decimals = source.Count();

            int midpoint = (decimals - 1) / 2;
            IEnumerable<float> sorted = source.OrderBy(n => n);
            
            float median = sorted.ElementAt(midpoint);
            if (decimals % 2 == 0)
                median = (median + sorted.ElementAt(midpoint + 1)) / 2;

            return median;
        }
    }
}