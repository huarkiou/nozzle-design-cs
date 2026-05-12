namespace Corelib.Tests;

using Corelib.Geometry;

public class RectangularTests
{
    [Fact]
    public void Rectangular_GeneratePoint_ZeroTheta_AxisAligned_ReturnsRightEdgeMidpoint()
    {
        // Axis-aligned rectangle (alpha=0): theta=0 -> right edge midpoint
        Rectangular r = new(0, 0, 4, 2, 0);
        Point p = r.GeneratePoint(0);
        Assert.Equal(2, p.X, 10);
        Assert.Equal(0, p.Y, 10);
    }

    [Fact]
    public void Rectangular_GeneratePoint_PiOver2_AxisAligned_ReturnsTopEdgeMidpoint()
    {
        Rectangular r = new(0, 0, 4, 2, 0);
        Point p = r.GeneratePoint(double.Pi / 2);
        Assert.Equal(0, p.X, 10);
        Assert.Equal(1, p.Y, 10);
    }

    [Fact]
    public void Rectangular_GeneratePoint_Pi_AxisAligned_ReturnsLeftEdgeMidpoint()
    {
        Rectangular r = new(0, 0, 4, 2, 0);
        Point p = r.GeneratePoint(double.Pi);
        Assert.Equal(-2, p.X, 10);
        Assert.Equal(0, p.Y, 10);
    }

    [Fact]
    public void Rectangular_GeneratePoints_ReturnsCountMultipleOfFour()
    {
        // Input 7 should be rounded up to 8 (next multiple of 4)
        Rectangular r = new(0, 0, 4, 2, 0);
        Point[] pts = r.GeneratePoints(7);
        Assert.Equal(8, pts.Length);
    }

    [Fact]
    public void Rectangular_GeneratePoints_ResultCountAlwaysMultipleOfFour()
    {
        // n = n - n%4 + 4 formula always adds 4 to round up
        Rectangular r = new(0, 0, 4, 2, 0);
        Point[] pts = r.GeneratePoints(12);
        // 12 - 0 + 4 = 16
        Assert.Equal(16, pts.Length);
        Assert.Equal(0, pts.Length % 4);
    }
}
