namespace Corelib.Geometry;

public class Ray2D
{
    public Point Origin { get; }
    public double Theta { get; }

    public Ray2D(Point origin, double theta)
    {
        Origin = origin;
        Theta = theta;
    }

    public Ray2D(Point from, Point to) : this(from, to.PolarAngleTo(from))
    {
    }

    public Point? Intersect(Segment2D segment, double epsilon = 1e-13)
    {
        Point? interPoint;

        double eq1 = Equation(segment.From);
        double eq2 = Equation(segment.To);

        if (double.Abs(eq1) < epsilon && double.Abs(eq2) < epsilon)
        {
            throw new ArgumentException("线段和射线重合，有无数个交点");
        }

        if (double.Abs(eq1) < epsilon)
        {
            interPoint = segment.From;
        }
        else if (double.Abs(eq2) < epsilon)
        {
            interPoint = segment.To;
        }
        else if (eq1 * eq2 < 0)
        {
            if (double.Abs(segment.From.X - segment.To.X) < epsilon && double.Abs(double.Cos(Theta)) >= epsilon)
            {
                // 线段垂直x轴
                interPoint = new Point(segment.From.X,
                    (segment.From.X - Origin.X) * double.Tan(Theta) + Origin.Y);
            }
            else if (double.Abs(double.Cos(Theta)) < epsilon &&
                     double.Abs(segment.From.X - segment.To.X) >= epsilon)
            {
                // 射线垂直x轴
                double k = (segment.To.Y - segment.From.Y) / (segment.To.X - segment.From.X);
                interPoint = new Point(Origin.X, k * (Origin.X - segment.From.X) + segment.From.Y);
            }
            else
            {
                double kSeg = (segment.To.Y - segment.From.Y) / (segment.To.X - segment.From.X);
                double kRay = double.Tan(Theta);
                double x = (Origin.Y - segment.From.Y + kSeg * segment.From.X - Origin.X * kRay) / (kSeg - kRay);
                double y = kSeg * (x - segment.From.X) + segment.From.Y;
                interPoint = new Point(x, y);
            }
        }
        else
        {
            interPoint = null;
        }

        if (interPoint is not null)
        {
            Point v = (Point)interPoint - Origin;
            // 判断交点是否在射线方向上
            if (Point.Dot(v, new Point(double.Cos(Theta), double.Sin(Theta))) < 0.0)
            {
                interPoint = null;
            }
        }

        return interPoint;
    }

    private double Equation(Point p)
    {
        return (p.X - Origin.X) * double.Sin(Theta) - (p.Y - Origin.Y) * double.Cos(Theta);
    }
}