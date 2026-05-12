namespace Corelib.Tests;

using Corelib.Geometry;

public class PolygonTests
{
    [Fact]
    public void Polygon_CalculatePolygonArea_SimpleTriangle_ReturnsCorrectValue()
    {
        // Triangle (0,0), (2,0), (0,2): actual area = 2
        // CalculatePolygonArea returns 2 * actual area = 4
        double area = Polygon.CalculatePolygonArea(
            new Point(0, 0), new Point(2, 0), new Point(0, 2));
        Assert.Equal(4, area, 10);
    }

    [Fact]
    public void Polygon_CalculatePolygonCentroid_SimpleTriangle_ReturnsCorrectValue()
    {
        // Triangle (0,0), (2,0), (0,2): centroid via code = (1/3, 1/3)
        Point centroid = Polygon.CalculatePolygonCentroid(
            new Point(0, 0), new Point(2, 0), new Point(0, 2));
        Assert.Equal(1.0 / 3.0, centroid.X, 10);
        Assert.Equal(1.0 / 3.0, centroid.Y, 10);
    }

    [Fact]
    public void Polygon_GeneratePoint_Square_ZeroTheta_ReturnsRightEdgeMidpoint()
    {
        // Square centered at origin, side 4, alpha=0
        Polygon poly = new(0, 0,
        [
            new Point(2, 2), new Point(-2, 2),
            new Point(-2, -2), new Point(2, -2)
        ], 0);

        Point p = poly.GeneratePoint(0);
        Assert.Equal(2, p.X, 10);
        Assert.Equal(0, p.Y, 10);
    }

    [Fact]
    public void Polygon_GeneratePoint_Square_PiOver2_ReturnsTopEdgeMidpoint()
    {
        Polygon poly = new(0, 0,
        [
            new Point(2, 2), new Point(-2, 2),
            new Point(-2, -2), new Point(2, -2)
        ], 0);

        Point p = poly.GeneratePoint(double.Pi / 2);
        Assert.Equal(0, p.X, 10);
        Assert.Equal(2, p.Y, 10);
    }

    [Fact]
    public void Polygon_GeneratePoints_ReturnsAtLeastNPoints()
    {
        Polygon poly = new(0, 0,
        [
            new Point(2, 2), new Point(-2, 2),
            new Point(-2, -2), new Point(2, -2)
        ], 0);

        Point[] pts = poly.GeneratePoints(4);
        Assert.True(pts.Length >= 4);
    }

    [Fact]
    public void Polygon_Constructor_FewerThan3Vertices_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Polygon(0, 0, [new Point(0, 0), new Point(1, 1)], 0));
    }
}
