namespace Corelib.Geometry;

public struct Point(double x, double y) : IEquatable<Point>
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;

    public Point() : this(0, 0)
    {
    }

    public override string ToString() => $"{{X={X},Y={Y}}}";

    public double[] ToArray => [X, Y];

    public bool Equals(Point other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point left, Point right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right)
    {
        return !left.Equals(right);
    }
}