namespace Corelib.Tests;

using Corelib.Geometry;

public class EllipseTests
{
    [Fact]
    public void Ellipse_GeneratePoint_ZeroTheta_AxisAligned_ReturnsRightmostPoint()
    {
        // Axis-aligned ellipse (alpha=0) at theta=0: point = (x0 + A, y0)
        Ellipse e = new(0, 0, 5, 3, 0);
        Point p = e.GeneratePoint(0);
        Assert.Equal(5, p.X, 5);
        Assert.Equal(0, p.Y, 5);
    }

    [Fact]
    public void Ellipse_GeneratePoint_PiOver2_AxisAligned_ReturnsTopPoint()
    {
        // Axis-aligned ellipse at theta=Pi/2: point = (x0, y0 + B)
        Ellipse e = new(0, 0, 5, 3, 0);
        Point p = e.GeneratePoint(double.Pi / 2);
        Assert.Equal(0, p.X, 5);
        Assert.Equal(3, p.Y, 5);
    }

    [Fact]
    public void Ellipse_GeneratePoint_Pi_AxisAligned_ReturnsLeftmostPoint()
    {
        // Axis-aligned ellipse at theta=Pi: point = (x0 - A, y0)
        Ellipse e = new(2, 1, 5, 3, 0);
        Point p = e.GeneratePoint(double.Pi);
        Assert.Equal(-3, p.X, 5);
        Assert.Equal(1, p.Y, 5);
    }

    [Fact]
    public void Ellipse_GeneratePoints_ReturnsRequestedCount()
    {
        Ellipse e = new(0, 0, 5, 3, 0);
        Point[] pts = e.GeneratePoints(20);
        Assert.Equal(20, pts.Length);
    }
}
