using Corelib.Geometry;

namespace GuiApp.ViewModels;

public interface IClosedCurveViewModel
{
    public IClosedCurve? GetClosedCurve();
    public string GetTomlString();
}