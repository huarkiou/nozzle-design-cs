using MathNet.Numerics.RootFinding;

namespace Corelib.Geometry;

public class Ellipse(double x0, double y0, double a, double b, double alpha = 0) : IClosedCurve
{
    public Point Center { get; } = new(x0, y0);
    public double A { get; } = a;
    public double B { get; } = b;
    public double Alpha { get; } = alpha; // rad

    public Ellipse(Point center, double a, double b, double alpha) : this(center.X, center.Y, a, b, alpha)
    {
    }

    public Point GeneratePoint(double theta)
    {
        var r = Broyden.FindRoot((arr) =>
        {
            double x = arr[0];
            return
            [
                double.Atan2(A * double.Cos(x) * double.Sin(Alpha) + B * double.Sin(x) * double.Cos(Alpha),
                    A * double.Cos(x) * double.Cos(Alpha) - B * double.Sin(x) * double.Sin(Alpha)) -
                theta
            ];
        }, [theta], 1e-13);
        double beta = r[0];
        return new Point(Center.X + A * double.Cos(beta) * double.Cos(Alpha) - B * double.Sin(beta) * double.Sin(Alpha),
            Center.Y + A * double.Cos(beta) * double.Sin(Alpha) + B * double.Sin(beta) * double.Cos(Alpha));
    }

    public Point[] GeneratePoints(int n)
    {
        var points = new Point[n];
        double deltaTheta = 2 * double.Pi / n;
        for (int i = 0; i < n; ++i)
        {
            double theta = i * deltaTheta;
            points[i].X = Center.X + A * double.Cos(theta) * double.Cos(Alpha) -
                          B * double.Sin(theta) * double.Sin(Alpha);
            points[i].Y = Center.Y + A * double.Cos(theta) * double.Sin(Alpha) +
                          B * double.Sin(theta) * double.Cos(Alpha);
        }

        return points;
    }
}