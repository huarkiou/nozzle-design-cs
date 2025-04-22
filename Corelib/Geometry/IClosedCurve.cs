namespace Corelib.Geometry;

public interface IClosedCurve
{
    /// <summary>
    /// 获取封闭曲线的中心点
    /// </summary>
    /// <returns>中心点</returns>
    public Point Center { get; }

    /// <summary>
    /// 从中心坐标作为原点的极坐标系看，极角为theta的射线与封闭曲线的唯一交点
    /// </summary>
    /// <param name="theta">极角</param>
    /// <returns>待求点</returns>
    public Point GeneratePoint(double theta);

    /// <summary>
    /// 根据采样点数n离散封闭曲线
    /// </summary>
    /// <param name="n">采样点数量，保证不少于n，但可能大于n</param>
    /// <returns>待求点数组，长度可能大于n</returns>
    public Point[] GeneratePoints(int n);
}