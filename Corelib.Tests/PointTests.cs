namespace Corelib.Tests;

using Corelib.Geometry;

public class PointTests
{
    [Fact]
    public void Point_DefaultConstructor_ReturnsZero()
    {
        Point p = new();
        Assert.Equal(0, p.X);
        Assert.Equal(0, p.Y);
    }

    [Fact]
    public void Point_Constructor_SetsCoordinates()
    {
        Point p = new(3.5, -2.5);
        Assert.Equal(3.5, p.X);
        Assert.Equal(-2.5, p.Y);
    }

    [Fact]
    public void Point_AdditionOperator_AddsComponents()
    {
        Point a = new(1, 2);
        Point b = new(3, 4);
        Point result = a + b;
        Assert.Equal(4, result.X);
        Assert.Equal(6, result.Y);
    }

    [Fact]
    public void Point_SubtractionOperator_SubtractsComponents()
    {
        Point a = new(5, 7);
        Point b = new(2, 3);
        Point result = a - b;
        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
    }

    [Fact]
    public void Point_MultiplyOperator_ScalarRight()
    {
        Point p = new(2, 3);
        Point result = p * 4;
        Assert.Equal(8, result.X);
        Assert.Equal(12, result.Y);
    }

    [Fact]
    public void Point_MultiplyOperator_ScalarLeft()
    {
        Point p = new(2, 3);
        Point result = 4 * p;
        Assert.Equal(8, result.X);
        Assert.Equal(12, result.Y);
    }

    [Fact]
    public void Point_DivisionOperator_DividesComponents()
    {
        Point p = new(8, 12);
        Point result = p / 2;
        Assert.Equal(4, result.X);
        Assert.Equal(6, result.Y);
    }

    [Fact]
    public void Point_Equals_SameValues_ReturnsTrue()
    {
        Point a = new(1.5, -3.0);
        Point b = new(1.5, -3.0);
        Assert.True(a == b);
        Assert.True(a.Equals(b));
        Assert.False(a != b);
        Assert.True(a.Equals((object)b));
    }

    [Fact]
    public void Point_Equals_DifferentValues_ReturnsFalse()
    {
        Point a = new(1, 2);
        Point b = new(1, 3);
        Assert.False(a == b);
        Assert.False(a.Equals(b));
        Assert.True(a != b);
    }

    [Fact]
    public void Point_GetHashCode_SameValues_SameHash()
    {
        Point a = new(2.5, 4.5);
        Point b = new(2.5, 4.5);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Point_Distance_CalculatesCorrectly()
    {
        Point a = new(0, 0);
        Point b = new(3, 4);
        double dist = Point.Distance(a, b);
        Assert.Equal(5, dist, 10);
    }

    [Fact]
    public void Point_DistanceTo_MatchesStaticDistance()
    {
        Point a = new(1, 1);
        Point b = new(4, 5);
        Assert.Equal(Point.Distance(a, b), a.DistanceTo(b), 10);
    }

    [Fact]
    public void Point_PolarAngleTo_CalculatesAngleFromOrigin()
    {
        Point origin = new(0, 0);
        Point p = new(1, 1);
        double angle = p.PolarAngleTo(origin);
        Assert.Equal(double.Pi / 4, angle, 10);
    }

    [Fact]
    public void Point_Rotate_RotatesAroundOriginByNinetyDegrees()
    {
        Point origin = new(0, 0);
        Point p = new(1, 0);
        Point rotated = p.Rotate(origin, double.Pi / 2);
        Assert.Equal(0, rotated.X, 10);
        Assert.Equal(1, rotated.Y, 10);
    }

    [Fact]
    public void Point_Dot_CalculatesDotProduct()
    {
        Point a = new(1, 2);
        Point b = new(3, 4);
        double dot = Point.Dot(a, b);
        Assert.Equal(11, dot, 10);
    }

    [Fact]
    public void Point_ToArray_ReturnsXYArray()
    {
        Point p = new(7, 13);
        double[] arr = p.ToArray;
        Assert.Equal(2, arr.Length);
        Assert.Equal(7, arr[0]);
        Assert.Equal(13, arr[1]);
    }

    [Fact]
    public void Point_ToString_ReturnsFormattedString()
    {
        Point p = new(1.5, 2.5);
        string s = p.ToString();
        Assert.Equal("{X=1.5,Y=2.5}", s);
    }
}
