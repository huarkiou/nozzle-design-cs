using MathNet.Numerics.RootFinding;

namespace Corelib.Geometry;

public class SuperEllipse(double x0, double y0, double a, double b, double power, double alpha = 0) : IClosedCurve
{
    public Point Center { get; } = new(x0, y0);
    public double A { get; } = a;
    public double B { get; } = b;
    public double Alpha { get; } = alpha; // rad
    public double Power { get; } = power; // rad

    public SuperEllipse(Point center, double a, double b, double power, double alpha)
        : this(center.X, center.Y, a, b, power, alpha)
    {
    }

    public Point GeneratePoint(double theta)
    {
        var r = Broyden.FindRoot((arr) =>
        {
            double x = arr[0];
            return
            [
                double.Atan2(A * double.Pow(double.Abs(double.Cos(x)), 2 / Power)
                               * double.Sign(double.Cos(x)) * double.Sin(Alpha) +
                             B * double.Pow(double.Abs(double.Sin(x)), 2 / Power)
                               * double.Sign(double.Sin(x)) * double.Cos(Alpha),
                    A * double.Pow(double.Abs(double.Cos(x)), 2 / Power)
                      * double.Sign(double.Cos(x)) * double.Cos(Alpha) -
                    B * double.Pow(double.Abs(double.Sin(x)), 2 / Power)
                      * double.Sign(double.Sin(x)) * double.Sin(Alpha)) - theta
            ];
        }, [theta], 1e-13);
        double beta = r[0];
        return new Point(x0 +
                         A * double.Pow(double.Abs(double.Cos(beta)), 2 / Power)
                           * double.Sign(double.Cos(beta)) * double.Cos(Alpha) -
                         B * double.Pow(double.Abs(double.Sin(beta)), 2 / Power)
                           * double.Sign(double.Sin(beta)) * double.Sin(Alpha),
            y0 +
            A * double.Pow(double.Abs(double.Cos(beta)), 2 / Power)
              * double.Sign(double.Cos(beta)) * double.Sin(Alpha) +
            B * double.Pow(double.Abs(double.Sin(beta)), 2 / Power)
              * double.Sign(double.Sin(beta)) * double.Cos(Alpha));
    }

    public Point[] GeneratePoints(int n)
    {
        var points = new Point[n];
        double deltaTheta = 2 * double.Pi / n;
        for (int i = 0; i < n; ++i)
        {
            double theta = i * deltaTheta;
            points[i].X = x0 +
                          A * double.Pow(double.Abs(double.Cos(theta)), 2 / Power)
                            * double.Sign(double.Cos(theta)) * double.Cos(Alpha) -
                          B * double.Pow(double.Abs(double.Sin(theta)), 2 / Power)
                            * double.Sign(double.Sin(theta)) * double.Sin(Alpha);
            points[i].Y = y0 +
                          A * double.Pow(double.Abs(double.Cos(theta)), 2 / Power)
                            * double.Sign(double.Cos(theta)) * double.Sin(Alpha) +
                          B * double.Pow(double.Abs(double.Sin(theta)), 2 / Power)
                            * double.Sign(double.Sin(theta)) * double.Cos(Alpha);
        }

        return points;
    }
}