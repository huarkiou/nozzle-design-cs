namespace Corelib.Geometry;

public class Polygon : IClosedCurve
{
    public Point Center { get; }
    public Point[] Vertices { get; }
    public double Alpha { get; }

    public Polygon(double x0, double y0, Point[] vertices, double alpha)
    {
        if (double.IsFinite(x0) && double.IsFinite(y0))
        {
            Center = new Point(x0, y0);
        }
        else
        {
            Center = CalculatePolygonCentroid(vertices);
        }

        if (vertices.Length < 3)
        {
            throw new ArgumentException("The polygon must have at least 3 vertices.");
        }
        else
        {
            Vertices = vertices;
        }

        Alpha = alpha;
    }

    public Polygon(Point center, Point[] vertices, double alpha) : this(center.X, center.Y, vertices, alpha)
    {
    }

    public Point GeneratePoint(double theta)
    {
        theta -= Alpha;
        for (int i1 = 0, i2 = 1; i1 < Vertices.Length; ++i1, ++i2)
        {
            i1 %= Vertices.Length;
            i2 %= Vertices.Length;
            Ray2D ray = new Ray2D(Center, theta);
            Segment2D segment = new Segment2D(Vertices[i1], Vertices[i2]);
            var interPoint = ray.Intersect(segment);
            if (interPoint.HasValue)
            {
                return interPoint.Value.Rotate(Center, Alpha);
            }
        }

        throw new ArgumentException("Polygon.GeneratePoint return null");
    }

    public Point[] GeneratePoints(int n)
    {
        var points = new SortedList<double, Point>();
        double deltaTheta = 2 * Math.PI / n;
        for (int i = 0; i < n; ++i)
        {
            double theta = deltaTheta * i;
            var p = GeneratePoint(theta);
            points.TryAdd(p.PolarAngleTo(Center), p);
        }

        foreach (var p in Vertices)
        {
            var tmp = p.Rotate(Center, Alpha);
            double polar = tmp.PolarAngleTo(Center);
            points.TryAdd(polar, tmp);
        }

        return points.Values.ToArray();
    }

    public static double CalculatePolygonArea(params Point[] points)
    {
        double area = 0;
        Point pPrev = points.Last();
        foreach (var p in points)
        {
            area += p.Y * pPrev.X - p.X * pPrev.Y;
            pPrev = p;
        }

        return double.Abs(area);
    }

    public static Point CalculatePolygonCentroid(params Point[] points)
    {
        double area = CalculatePolygonArea(points);
        double x0 = 0, y0 = 0;
        Point pPrev = points.Last();
        foreach (var p in points)
        {
            double tmp = p.Y * pPrev.X - pPrev.Y * p.X;
            x0 += (p.X + pPrev.X) * tmp;
            y0 += (p.Y + pPrev.Y) * tmp;
            pPrev = p;
        }

        return new Point(x0 / (6 * area), y0 / (6 * area));
    }
}