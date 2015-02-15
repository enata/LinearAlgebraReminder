using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinearAlgebraReminder.Utility;

namespace LinearAlgebraReminder
{
    public abstract class FlexibleSourceVector : IVector
    {
        public abstract double this[int i] { get; }

        public abstract int Size { get; }

        /// <summary>
        /// Euclidian length
        /// </summary>
        public double Length
        {
            get
            {
                // scale vector to avoid overflow
                double scalingFactor = this.Max(value => Math.Abs(value));
                if (scalingFactor.IsAbout(0.0))
                    return 0.0;
                IVector scaledVector = ScaleBy(1.0/scalingFactor);
                double sqrSum = scaledVector.Sum(value => value * value);
                double result = Math.Sqrt(sqrSum) * scalingFactor;
                return result;
            }
        }

        /// <summary>
        /// Calculates dot product of two vectors
        /// </summary>
        /// <param name="other">second dot product operand</param>
        /// <returns>dot product value</returns>
        public double Dot(IVector other)
        {
            if(other == null)
                throw new LinearAlgebraReminderException("Other vector shouldn't be null");
            if (other.Size != Size)
                throw new LinearAlgebraReminderException("Vectors should have the same size for dot product operation");
            double result = 0.0;
            for (int i = 0; i < Size; i++)
            {
                result += this[i]*other[i];
            }
            return result;
        }

        protected abstract FlexibleSourceVector CreateVector(double[] values);

        public IVector ScaleBy(double scalar)
        {
            // Multiplying vector x by scalar alpha yields a new vector in the same direction as x, but scaled by
            // a factor alpha
            var result = new Double[Size];
            for (int i = 0; i < Size; i++)
                result[i] = this[i] * scalar;
            return CreateVector(result);
        }

        public IVector Minus(IVector vector2)
        {
            if (vector2 == null)
                throw new LinearAlgebraReminderException("Cannot substract null vectors");
            if (Size != vector2.Size)
                throw new LinearAlgebraReminderException("Vectors sizes should be equal");

            //The vectors are added element-wise, yielding a new vector of the same size
            int resultSize = Size;
            var result = new Double[Size];
            for (int i = 0; i < resultSize; i++)
                result[i] = this[i] - vector2[i];
            return CreateVector(result);
        }

        public IVector Plus(IVector vector2)
        {
            if (vector2 == null)
                throw new LinearAlgebraReminderException("Cannot add null vectors");
            if (Size != vector2.Size)
                throw new LinearAlgebraReminderException("Vectors sizes should be equal");

            //The vectors are added element-wise, yielding a new vector of the same size
            int resultSize = Size;
            var result = new Double[Size];
            for (int i = 0; i < resultSize; i++)
                result[i] = this[i] + vector2[i];
            return CreateVector(result);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (FlexibleSourceVector) obj;

            //Two vectors are equal if they point in the same direction and are of the same length
            if (other.Size != Size)
                return false;
            for(int i = 0; i < Size; i++)
                if (! this[i].IsAbout(other[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return this.Aggregate(19, (current, value) => current*31 + value.GetHashCode());
        }

        public abstract IEnumerator<double> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
