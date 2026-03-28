using FluentAAS.Builder;

namespace FluentAAS.Templates.TechnicalData;

/// <summary>
/// Provides a ready-to-run composition sample for the Technical Data template.
/// The example demonstrates how to build a complete AAS environment with realistic motor and bearing data.
/// </summary>
public static class TechnicalDataCompositionExample
{
    /// <summary>
    /// Builds an example <see cref="IEnvironment"/> containing one shell and one Technical Data submodel.
    /// Integrators can use this as a reference implementation for production composition flows.
    /// </summary>
    /// <returns>A fully built AAS environment with technical parameter groups.</returns>
    public static IEnvironment BuildMotorAndBearingExampleEnvironment()
    {
        var aasBuilder = AasBuilder.Create();

        aasBuilder.AddShell("urn:aas:example:drivetrain:1000", "DriveTrainAAS")
                  .AddTechnicalData("urn:aas:submodel:technical-data:drivetrain:1000")
                  .WithMotorPerformance(motor => motor
                      .WithRatedVoltage(400m, "V", TechnicalDataSemantics.RatedVoltage)
                      .WithRatedCurrent(82.5m, "A", TechnicalDataSemantics.RatedCurrent)
                      .WithRatedPower(45m, "kW", TechnicalDataSemantics.RatedPower)
                      .WithRatedSpeed(1470m, "1/min", TechnicalDataSemantics.RatedSpeed))
                  .WithBearingCharacteristics(bearing => bearing
                      .WithInnerDiameter(50m, "mm", TechnicalDataSemantics.InnerDiameter)
                      .WithOuterDiameter(90m, "mm", TechnicalDataSemantics.OuterDiameter)
                      .WithWidth(20m, "mm", TechnicalDataSemantics.Width)
                      .WithLimitingSpeed(6500m, "1/min", TechnicalDataSemantics.LimitingSpeed))
                  .BuildTechnicalData();

        return aasBuilder.Build();
    }
}
