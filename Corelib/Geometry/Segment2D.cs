namespace Corelib.Geometry;

public class Segment2D(Point from, Point to)
{
    public Point From { get; } = from;
    public Point To { get; } = to;

    public double Theta => double.Atan2(To.Y - From.Y, To.X - From.X);
    public double Length => Point.Distance(From, To);
}