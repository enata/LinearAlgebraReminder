using System.Collections.Generic;

namespace LinearAlgebraReminder
{
    public interface IVector : IEnumerable<double>
    {
        double this[int i] { get; }
        int Size { get; }
        double Dot(IVector other);
        IVector ScaleBy(double scalar);
        IVector Minus(IVector vector2);
        IVector Plus(IVector vector2);
    }
}
