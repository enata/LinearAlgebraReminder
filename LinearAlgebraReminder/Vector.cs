using System.Collections.Generic;
using LinearAlgebraReminder.Utility;

namespace LinearAlgebraReminder
{
    public sealed class Vector : FlexibleSourceVector
    {
        public static Vector CreateUnitBasis(int size, int nonZeroIndex)
        {
            if (nonZeroIndex >= size)
                throw new LinearAlgebraReminderException("Index exceeds vector dimensionality");
            if (nonZeroIndex < 0)
                throw new LinearAlgebraReminderException("Index is less than zero");
            var result = new Vector(size);
            result._values[nonZeroIndex] = 1.0;
            return result;
        }

        public Vector()
        {
            _values = new List<double>();
        }

        public Vector(int size)
        {
            if (size < 0)
                throw new LinearAlgebraReminderException("Vector size is less than zero");
            _values = new List<double>(size);
            for(int i = 0; i < size; i++)
                _values.Add(0.0);
        }

        public Vector(IEnumerable<double> values)
        {
            _values = new List<double>(values);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">vector to copy</param>
        public Vector(Vector other)
        {
            if (other == null)
                throw new LinearAlgebraReminderException("Vector shouldn't be null");
            _values = new List<double>(other._values);
        }

        /// <summary>
        /// Creates new vector between two points
        /// </summary>
        /// <param name="point1">start point</param>
        /// <param name="point2">end point</param>
        public Vector(Point point1, Point point2)
        {
            if (point1.Dimensionality != point2.Dimensionality)
                throw new LinearAlgebraReminderException("Points should have the same dimensionality");
            int vectorDimensionality = point1.Dimensionality;
            _values = new List<double>(vectorDimensionality);
            for (int i = 0; i < vectorDimensionality; i++)
            {
                _values[i] = point2[i] - point1[i];
            }
        }

        public override double this[int i]
        {
            get
            {
                if (i < 0)
                    throw new LinearAlgebraReminderException("Index is less than zero");
                if (i >= _values.Count)
                    throw new LinearAlgebraReminderException("Index out of range");
                return _values[i];
            }
        }

        public void Add(double value)
        {
            _values.Add(value);
        }

        public override int Size { get { return _values.Count; }}


        public override IEnumerator<double> GetEnumerator()
        {
            return _values.GetEnumerator();
        }


        protected override FlexibleSourceVector CreateVector(double[] values)
        {
            return new Vector(values);
        }

        private readonly List<double> _values;
    }
}
