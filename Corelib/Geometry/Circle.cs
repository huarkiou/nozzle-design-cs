namespace Corelib.Geometry;

public class Circle(double x0, double y0, double radius) : IClosedCurve
{
    public Point Center { get; } = new(x0, y0);
    public double Radius { get; } = radius;

    public Circle() : this(0, 0, 0)
    {
    }

    public Circle(Point center, double radius) : this(center.X, center.Y, radius)
    {
    }

    public Point GeneratePoint(double theta)
    {
        return new Point(Center.X + Radius * double.Cos(theta), Center.Y + Radius * double.Sin(theta));
    }

    public Point[] GeneratePoints(int n)
    {
        var points = new Point[n];
        double deltaTheta = 2 * double.Pi / n;
        for (int i = 0; i < n; ++i)
        {
            double theta = i * deltaTheta;
            points[i].X = Center.X + Radius * double.Cos(theta);
            points[i].Y = Center.Y + Radius * double.Sin(theta);
        }

        return points;
    }
}