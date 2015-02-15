using System.Collections.Generic;

namespace LinearAlgebraReminder
{
    public static class MatrixExtensions
    {
        public static Matrix ToMatrix(this IEnumerable<IVector> columns)
        {
            return new Matrix(columns);
        }

        public static IVector ToVector(this IEnumerable<double> values)
        {
            return new Vector(values);
        }
    }
}
