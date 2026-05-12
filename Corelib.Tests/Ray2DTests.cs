namespace Corelib.Tests;

using Corelib.Geometry;

public class Ray2DTests
{
    [Fact]
    public void Ray2D_Constructor_WithOriginAndTheta_SetsProperties()
    {
        Point origin = new(1, 2);
        Ray2D ray = new(origin, double.Pi / 4);
        Assert.Equal(1, ray.Origin.X);
        Assert.Equal(2, ray.Origin.Y);
        Assert.Equal(double.Pi / 4, ray.Theta);
    }

    [Fact]
    public void Ray2D_Constructor_WithFromAndTo_SetsThetaFromPoints()
    {
        Point from = new(0, 0);
        Point to = new(1, 1);
        Ray2D ray = new(from, to);
        Assert.Equal(0, ray.Origin.X);
        Assert.Equal(0, ray.Origin.Y);
        Assert.Equal(double.Pi / 4, ray.Theta, 10);
    }

    [Fact]
    public void Ray2D_Intersect_Hit_ReturnsIntersectionPoint()
    {
        // Ray from (0,0) going right (theta=0), segment from (1,-1) to (1,1)
        Ray2D ray = new(new Point(0, 0), 0);
        Segment2D seg = new(new Point(1, -1), new Point(1, 1));
        Point? result = ray.Intersect(seg);
        Assert.True(result.HasValue);
        Assert.Equal(1, result.Value.X, 10);
        Assert.Equal(0, result.Value.Y, 10);
    }

    [Fact]
    public void Ray2D_Intersect_Miss_ReturnsNull()
    {
        // Ray from (0,0) going right (theta=0), parallel segment above at y=1
        Ray2D ray = new(new Point(0, 0), 0);
        Segment2D seg = new(new Point(-2, 1), new Point(2, 1));
        Point? result = ray.Intersect(seg);
        Assert.False(result.HasValue);
    }

    [Fact]
    public void Ray2D_Intersect_BehindRay_ReturnsNull()
    {
        // Ray from (0,0) going right (theta=0), segment at x=-1 (behind ray)
        Ray2D ray = new(new Point(0, 0), 0);
        Segment2D seg = new(new Point(-1, -1), new Point(-1, 1));
        Point? result = ray.Intersect(seg);
        Assert.False(result.HasValue);
    }

    [Fact]
    public void Ray2D_Intersect_Collinear_ThrowsArgumentException()
    {
        // Ray from (0,0) going right (theta=0), segment lies on the same line
        Ray2D ray = new(new Point(0, 0), 0);
        Segment2D seg = new(new Point(1, 0), new Point(3, 0));
        Assert.Throws<ArgumentException>(() => ray.Intersect(seg));
    }
}

public class Segment2DTests
{
    [Fact]
    public void Segment2D_Constructor_SetsEndpoints()
    {
        Point from = new(1, 2);
        Point to = new(4, 6);
        Segment2D seg = new(from, to);
        Assert.Equal(1, seg.From.X);
        Assert.Equal(2, seg.From.Y);
        Assert.Equal(4, seg.To.X);
        Assert.Equal(6, seg.To.Y);
    }

    [Fact]
    public void Segment2D_Length_CalculatesCorrectly()
    {
        Segment2D seg = new(new Point(0, 0), new Point(3, 4));
        Assert.Equal(5, seg.Length, 10);
    }

    [Fact]
    public void Segment2D_Theta_CalculatesAngle()
    {
        Segment2D seg = new(new Point(0, 0), new Point(1, 1));
        Assert.Equal(double.Pi / 4, seg.Theta, 10);
    }
}
