namespace Corelib.Tests;

using Corelib.Geometry;

public class SuperEllipseTests
{
    [Fact]
    public void SuperEllipse_GeneratePoint_ReturnsPoint()
    {
        // Power=2 gives a regular ellipse; test that it produces a point
        SuperEllipse se = new(0, 0, 5, 3, 2, 0);
        Point p = se.GeneratePoint(0);
        Assert.True(double.IsFinite(p.X));
        Assert.True(double.IsFinite(p.Y));
    }

    [Fact]
    public void SuperEllipse_GeneratePoints_ReturnsRequestedCount()
    {
        SuperEllipse se = new(0, 0, 5, 3, 2, 0);
        Point[] pts = se.GeneratePoints(12);
        Assert.Equal(12, pts.Length);
    }

    [Fact]
    public void SuperEllipse_GeneratePoint_PiOver2_ReturnsPointOnCurve()
    {
        // At power=2 (ellipse), theta=Pi/2 should give (0, B)
        SuperEllipse se = new(0, 0, 5, 3, 2, 0);
        Point p = se.GeneratePoint(double.Pi / 2);
        Assert.Equal(0, p.X, 5);
        Assert.Equal(3, p.Y, 5);
    }
}
