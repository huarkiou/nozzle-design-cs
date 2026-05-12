namespace Corelib.Tests;

using Corelib.Geometry;

public class CircleTests
{
    [Fact]
    public void Circle_Constructor_WithCenterCoordinates_SetsProperties()
    {
        Circle c = new(1, 2, 5);
        Assert.Equal(1, c.Center.X);
        Assert.Equal(2, c.Center.Y);
        Assert.Equal(5, c.Radius);
    }

    [Fact]
    public void Circle_Constructor_WithPointCenter_SetsProperties()
    {
        Point center = new(3, 4);
        Circle c = new(center, 7);
        Assert.Equal(3, c.Center.X);
        Assert.Equal(4, c.Center.Y);
        Assert.Equal(7, c.Radius);
    }

    [Fact]
    public void Circle_GeneratePoint_ZeroTheta_ReturnsRightmostPoint()
    {
        Circle c = new(0, 0, 5);
        Point p = c.GeneratePoint(0);
        Assert.Equal(5, p.X, 10);
        Assert.Equal(0, p.Y, 10);
    }

    [Fact]
    public void Circle_GeneratePoint_PiOver2_ReturnsTopPoint()
    {
        Circle c = new(2, 3, 4);
        Point p = c.GeneratePoint(double.Pi / 2);
        Assert.Equal(2, p.X, 10);
        Assert.Equal(7, p.Y, 10);
    }

    [Fact]
    public void Circle_GeneratePoint_Pi_ReturnsLeftmostPoint()
    {
        Circle c = new(0, 0, 3);
        Point p = c.GeneratePoint(double.Pi);
        Assert.Equal(-3, p.X, 10);
        Assert.Equal(0, p.Y, 10);
    }

    [Fact]
    public void Circle_GeneratePoints_ReturnsRequestedCount()
    {
        Circle c = new(0, 0, 10);
        Point[] pts = c.GeneratePoints(8);
        Assert.Equal(8, pts.Length);
    }

    [Fact]
    public void Circle_GeneratePoints_AdjacentPointsAreEvenlySpaced()
    {
        Circle c = new(0, 0, 5);
        Point[] pts = c.GeneratePoints(100);
        // Adjacent mid-array points should be one angular step apart
        double angleA = pts[10].PolarAngleTo(c.Center);
        double angleB = pts[11].PolarAngleTo(c.Center);
        double expectedStep = 2 * double.Pi / 100;
        double diff = angleB - angleA;
        Assert.Equal(expectedStep, diff, 9);
    }
}
