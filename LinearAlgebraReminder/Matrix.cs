using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinearAlgebraReminder.Utility;

namespace LinearAlgebraReminder
{
    public sealed class Matrix : IEnumerable<IVector>
    {
        /// <summary>
        /// Generates new identity matrix
        /// </summary>
        /// <param name="size">matrix size</param>
        public static Matrix Identity(int size)
        {
            if (size <= 0)
                throw new LinearAlgebraReminderException("Invalid identity matrix size");

            var result = new Matrix(size, size);
            for (int i = 0; i < size; i++)
                result[i, i] = 1.0;
            return result;
        }

        public Matrix(int rowsNumber, int columnsNumber)
        {
            if(rowsNumber < 0)
                throw new LinearAlgebraReminderException("Rows number should be >= 0");
            if(columnsNumber < 0)
                throw new LinearAlgebraReminderException("Columns number should be >= 0");

            _values = new double[rowsNumber, columnsNumber];
            _columns = new ColumnCollection(_values);
            _rows = new RowCollection(_values);
        }

        public Matrix(double[,] values)
        {
            if(values == null)
                throw new LinearAlgebraReminderException("Null values argument");

            _values = (double[,]) values.Clone();
            _columns = new ColumnCollection(_values);
            _rows = new RowCollection(_values);
        }

        public Matrix(IEnumerable<IVector> columns)
        {
            if(columns == null)
                throw new LinearAlgebraReminderException("Null columns argument");

            var enumerated = columns as IVector[] ?? columns.ToArray();
            if (enumerated.Length == 0)
            {
                _values = new double[0, 0];
                _columns = new ColumnCollection(_values);
                _rows = new RowCollection(_values);
                return;
            }
            int rowsCount = enumerated[0].Size;
            int columnsCount = enumerated.Length;
            _values = new double[rowsCount, columnsCount];
            for (int i = 0; i < columnsCount; i++)
                for (int j = 0; j < rowsCount; j++)
                {
                    _values[j, i] = enumerated[i][j];
                }
            _columns = new ColumnCollection(_values);
            _rows = new RowCollection(_values);
        }

        public double this[int i, int j]
        {
            get { return _values[i, j]; }
            set { _values[i, j] = value; }
        }

        /// <summary>
        /// Generates transposed matrix
        /// </summary>
        public Matrix Transpose()
        {
            var result = _rows.ToMatrix();
            return result;
        }

        /// <summary>
        /// Scales matrix by a factor
        /// </summary>
        /// <param name="scalar">scaling factor</param>
        /// <returns>new matrix (original values scaled by factor)</returns>
        public Matrix Scale(double scalar)
        {
            var result = _columns.Select(column => column.ScaleBy(scalar)).ToMatrix();
            return result;
        }

        /// <summary>
        /// Adds two matrices
        /// </summary>
        /// <returns>new matrix (addition result)</returns>
        public Matrix Plus(Matrix matrix)
        {
            if(matrix == null)
                throw new LinearAlgebraReminderException("Null matrix argument");
            if(matrix._columns.Count != _columns.Count || matrix._rows.Count != _rows.Count)
                throw new LinearAlgebraReminderException("Invalid matrix dimensionality");

            var result = _columns.Zip(matrix._columns, (column1, column2) => column1.Plus(column2)).ToMatrix();
            return result;
        }

        /// <summary>
        /// Multiply matrice by vector
        /// </summary>
        /// <returns>new vector (multiplication result)</returns>
        public IVector Multiply(IVector vector)
        {
            if(vector == null)
                throw new LinearAlgebraReminderException("Null vector argument");
            if(_columns.Count != vector.Size)
                throw new LinearAlgebraReminderException("Inconsistent dimensionality");

            var result = _rows.Select(row => row.Dot(vector)).ToVector();
            return result;
        }

        public IEnumerator<IVector> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IVectorCollection Columns
        {
            get { return _columns; }
        }

        public IVectorCollection Rows
        {
            get { return _rows; }
        }

        private readonly double[,] _values;
        private readonly ColumnCollection _columns;
        private readonly RowCollection _rows;

        public interface IVectorCollection : IEnumerable<IVector>
        {
            IVector this[int i] { get; set; }
        }

        private abstract class VectorCollection : IVectorCollection
        {
            protected VectorCollection()
            {
                Elements = new List<MatrixPartVector>();
            }

            public IVector this[int i]
            {
                get
                {
                    if (i >= Elements.Count || i < 0)
                        throw new LinearAlgebraReminderException("Index out of range");

                    return Elements[i];
                }
                set
                {
                    if (i >= Elements.Count || i < 0)
                        throw new LinearAlgebraReminderException("Index out of range");
                    if (value == null)
                        throw new LinearAlgebraReminderException("Can't assign null value");
                    if (value.Size != Elements[i].Size)
                        throw new LinearAlgebraReminderException("Invalid vector size");

                    for (int j = 0; j < value.Size; j++)
                    {
                        Elements[i].Set(j, value[j]);
                    }
                }
            }

            public int Count
            {
                get { return Elements.Count; }
            }

            public IEnumerator<IVector> GetEnumerator()
            {
                return Elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            protected readonly List<MatrixPartVector> Elements;     
        }


        private class ColumnCollection : VectorCollection
        {
            public ColumnCollection(double[,] matrixValues)
            {
                int columnsNumber = matrixValues.GetLength(1);
                for(int i = 0; i < columnsNumber; i++)
                    Elements.Add(new MatrixColumn(i, matrixValues));
            }
        }

        private class RowCollection : VectorCollection
        {
            public RowCollection(double[,] matrixValues)
            {
                int rowsNumber = matrixValues.GetLength(0);
                for (int i = 0; i < rowsNumber; i++)
                    Elements.Add(new MatrixRow(i, matrixValues));
            }
        }

        private abstract class MatrixPartVector : FlexibleSourceVector
        {
            protected MatrixPartVector(int index, double[,] matrixValues)
            {
                Index = index;
                MatrixValues = matrixValues;
            }

            protected override FlexibleSourceVector CreateVector(double[] values)
            {
                return new Vector(values);
            }

            public override IEnumerator<double> GetEnumerator()
            {
                for (int i = 0; i < Size; i++)
                    yield return this[i];
            }

            public abstract void Set(int index, double value);

            protected readonly int Index;
            protected readonly double[,] MatrixValues;
        }

        private class MatrixColumn : MatrixPartVector
        {
            public MatrixColumn(int index, double[,] matrixValues)
                : base(index, matrixValues)
            {
            }

            public override double this[int i]
            {
                get { return MatrixValues[i, Index]; }
            }


            public override int Size
            {
                get { return MatrixValues.GetLength(0); }
            }

            public override void Set(int index, double value)
            {
                MatrixValues[index, Index] = value;
            }
        }

        private class MatrixRow : MatrixPartVector
        {
            public MatrixRow(int index, double[,] matrixValues)
                : base(index, matrixValues)
            {
            }

            public override double this[int i]
            {
                get { return MatrixValues[Index, i]; }
            }

            public override int Size
            {
                get { return MatrixValues.GetLength(1); }
            }

            public override void Set(int index, double value)
            {
                MatrixValues[Index, index] = value;
            }
        }
    }
}
