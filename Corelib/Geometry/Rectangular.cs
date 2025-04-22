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
        throw new NotImplementedException();
    }

    public Point[] GeneratePoints(int n)
    {
        n = n - n % 4 + 4; //< 现在N是四的倍数
        var points = new Point[n];

        // 求四个角点坐标
        Point p1 = new Point(x0 - Length / 2.0 * double.Cos(Alpha) - Width / 2.0 * double.Sin(Alpha),
            y0 - Length / 2.0 * double.Sin(Alpha) + Width / 2.0 * double.Cos(Alpha));
        Point p2 = new Point(x0 + Length / 2.0 * double.Cos(Alpha) - Width / 2.0 * double.Sin(Alpha),
            y0 + Length / 2.0 * double.Sin(Alpha) + Width / 2.0 * double.Cos(Alpha));
        Point p3 = new Point(x0 + Length / 2.0 * double.Cos(Alpha) + Width / 2.0 * double.Sin(Alpha),
            y0 + Length / 2.0 * double.Sin(Alpha) - Width / 2.0 * double.Cos(Alpha));
        Point p4 = new Point(x0 - Length / 2.0 * double.Cos(Alpha) + Width / 2.0 * double.Sin(Alpha),
            y0 - Length / 2.0 * double.Sin(Alpha) - Width / 2.0 * double.Cos(Alpha));

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
}