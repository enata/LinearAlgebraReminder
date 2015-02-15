using System.Collections.Generic;

namespace LinearAlgebraReminder
{
    public class Point : IEnumerable<double>
    {
        public Point(params double[] coordinates)
        {
            _coordinates = new List<double>(coordinates);
        }

        public double this[int i]
        {
            get { return _coordinates[i]; }
        }

        public int Dimensionality
        {
            get { return _coordinates.Count; }
        }

        public IEnumerator<double> GetEnumerator()
        {
            return _coordinates.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<double> _coordinates;
    }
}
