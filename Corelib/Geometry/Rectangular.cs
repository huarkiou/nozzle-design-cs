namespace Corelib.Geometry;

public class Rectangular(double x0, double y0, double length, double width, double alpha = 0) : IClosedCurve
{
    public Point Center { get; } = new(x0, y0);
    public double Length { get; } = length;
    public double Width { get; } = width;
    public double Alpha { get; } = alpha;

    public Rectangular(Point center, double length, double width, double alpha) : this(center.X, center.Y, length,
        width, alpha)
    {
    }

    public Point GeneratePoint(double theta)
    {
        // 求四个角点坐标
        var (p1, p2, p3, p4) = GetCornerPoints();
        Segment2D left = new Segment2D(p1, p2);
        Segment2D top = new Segment2D(p2, p3);
        Segment2D right = new Segment2D(p3, p4);
        Segment2D bottom = new Segment2D(p4, p1);
        Ray2D ray = new Ray2D(Center, theta);
        var interPoint = ray.Intersect(left);
        if (interPoint.HasValue)
        {
            return interPoint.Value;
        }

        interPoint = ray.Intersect(top);
        if (interPoint.HasValue)
        {
            return interPoint.Value;
        }

        interPoint = ray.Intersect(right);
        if (interPoint.HasValue)
        {
            return interPoint.Value;
        }

        interPoint = ray.Intersect(bottom);
        if (interPoint.HasValue)
        {
            return interPoint.Value;
        }

        if (interPoint is null)
        {
            throw new Exception("Intersect point is null");
        }
        else
        {
            return interPoint.Value;
        }
    }

    public Point[] GeneratePoints(int n)
    {
        n = n - n % 4 + 4; //< 现在N是四的倍数
        var points = new Point[n];

        // 求四个角点坐标
        var (p1, p2, p3, p4) = GetCornerPoints();

        int num = n / 4;
        for (int i = 0; i < num; ++i)
        {
            points[i + 0 * num] = p1 + (double)i / num * (p2 - p1);
            points[i + 1 * num] = p2 + (double)i / num * (p3 - p2);
            points[i + 2 * num] = p3 + (double)i / num * (p4 - p3);
            points[i + 3 * num] = p4 + (double)i / num * (p1 - p4);
        }

        return points;
    }

    private (Point lb, Point lu, Point ru, Point rb) GetCornerPoints()
    {
        Point p1 = new Point(x0 - Length / 2.0 * double.Cos(Alpha) - Width / 2.0 * double.Sin(Alpha),
            y0 - Length / 2.0 * double.Sin(Alpha) + Width / 2.0 * double.Cos(Alpha));
        Point p2 = new Point(x0 + Length / 2.0 * double.Cos(Alpha) - Width / 2.0 * double.Sin(Alpha),
            y0 + Length / 2.0 * double.Sin(Alpha) + Width / 2.0 * double.Cos(Alpha));
        Point p3 = new Point(x0 + Length / 2.0 * double.Cos(Alpha) + Width / 2.0 * double.Sin(Alpha),
            y0 + Length / 2.0 * double.Sin(Alpha) - Width / 2.0 * double.Cos(Alpha));
        Point p4 = new Point(x0 - Length / 2.0 * double.Cos(Alpha) + Width / 2.0 * double.Sin(Alpha),
            y0 - Length / 2.0 * double.Sin(Alpha) - Width / 2.0 * double.Cos(Alpha));
        return (p1, p2, p3, p4);
    }
}