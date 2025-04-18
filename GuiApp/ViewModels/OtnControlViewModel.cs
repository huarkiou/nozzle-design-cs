using System;
using CommunityToolkit.Mvvm.Input;

namespace GuiApp.ViewModels;

public partial class OtnControlViewModel : ViewModelBase
{
    // MOC Control
    public bool Irrotational { get; set; } = true;
    public bool IsAxisymmetric { get; set; } = true;
    public double Epsilon { get; set; } = 1e-6;
    public int NumCorrectionMax { get; set; } = 40;
    public int NumInletDivision { get; set; } = 101;

    // Geometry
    public double InletHeight { get; set; } = 1.0;
    public double Length { get; set; } = 4.0;
    public double Width { get; set; } = 1.0;
    public double TargetOutletHeight { get; set; } = double.NaN;

    // Inlet
    public double Gamma { get; set; } = 1.4;
    public double Rg { get; set; } = 287.042;
    public double TotalPressure { get; set; } = 800000.0;
    public double TotalTemperature { get; set; } = 2000.0;
    public double MachNumber { get; set; } = 1.20;
    public double InletTheta { get; set; } = 0.0;

    // Throat
    public double RadiusThroat { get; set; } = 0.0;
    public double InitialExpansionAngle { get; set; } = double.NaN;

    // Outlet
    public double PressureAmbient { get; set; } = 7000.0;


    [RelayCommand]
    public void RunOtn()
    {
        Gamma = 1.3;
        Console.WriteLine(Length);
    }
}