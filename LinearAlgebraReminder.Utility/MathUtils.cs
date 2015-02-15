using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebraReminder.Utility
{
    public static class MathUtils
    {
        public static bool IsAbout(this double number1, double number2, double precision = Double.Epsilon)
        {
            return Math.Abs(number2 - number1) <= precision;
        }
    }
}
