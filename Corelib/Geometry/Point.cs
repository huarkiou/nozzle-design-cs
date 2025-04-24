namespace Corelib.Geometry;

public struct Point(double x, double y) : IEquatable<Point>
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;

    public Point() : this(0, 0)
    {
    }

    public static Point operator +(Point left, Point right)
    {
        return new Point(left.X + right.X, left.Y + right.Y);
    }

    public static Point operator -(Point left, Point right)
    {
        return new Point(left.X - right.X, left.Y - right.Y);
    }

    public static Point operator *(Point left, double right)
    {
        return new Point(left.X * right, left.Y * right);
    }

    public static Point operator *(double left, Point right)
    {
        return new Point(left * right.X, left * right.Y);
    }

    public static Point operator /(Point left, double right)
    {
        return new Point(left.X / right, left.Y / right);
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

    public static double Distance(Point p1, Point p2)
    {
        return double.Sqrt(double.Pow(p1.X - p2.X, 2) + double.Pow(p1.Y - p2.Y, 2));
    }

    public double DistanceTo(Point other)
    {
        return Distance(this, other);
    }

    public double PolarAngleTo(Point origin)
    {
        return double.Atan2(Y - origin.Y, X - origin.X);
    }

    public static double Dot(Point lhs, Point rhs)
    {
        return lhs.X * rhs.X + lhs.Y * rhs.Y;
    }

    public void RotateInPlace(Point origin, double angle)
    {
        double theta = PolarAngleTo(origin);
        double l = DistanceTo(origin);
        X = origin.X + l * Math.Cos(theta + angle);
        Y = origin.Y + l * Math.Sin(theta + angle);
    }

    public Point Rotate(Point origin, double angle)
    {
        double theta = PolarAngleTo(origin);
        double l = DistanceTo(origin);
        double x = origin.X + l * Math.Cos(theta + angle);
        double y = origin.Y + l * Math.Sin(theta + angle);
        return new Point(x, y);
    }
}

public static class PointExtensions
{
    public static Point[] LoadTxt(string fileName)
    {
        List<Point> result = [];
        foreach (string line in File.ReadLines(fileName))
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith('#') || trimmedLine.Length == 0)
            {
                continue;
            }

            var r = trimmedLine.Split(' ', ',', '\t');
            if (r.Length > 1)
            {
                double x = double.Parse(r[0]);
                double y = double.Parse(r[1]);
                result.Add(new Point(x, y));
            }
        }

        return result.ToArray();
    }

    public static List<Point[]> LoadDat(string fileName)
    {
        List<Point[]> ret = [];
        List<Point> result = [];
        foreach (string line in File.ReadLines(fileName))
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith('#') || trimmedLine.Length == 0)
            {
                continue;
            }

            if (trimmedLine.Contains("ROW"))
            {
                if (result.Count > 0)
                {
                    ret.Add(result.ToArray());
                    result = [];
                }
            }
            else
            {
                var r = trimmedLine.Split(' ', ',', '\t');
                if (r.Length > 1)
                {
                    double x = double.Parse(r[0]);
                    double y = double.Parse(r[1]);
                    result.Add(new Point(x, y));
                }
            }
        }

        return ret;
    }
}