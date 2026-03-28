using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAAS.Templates.TechnicalData;

namespace FluentAASTests.Integration;

/// <summary>
/// Test-only composition sample for the Technical Data template.
/// Kept in the integration test project so production packages stay free of example-only code.
/// </summary>
public static class TechnicalDataCompositionExample
{
    /// <summary>
    /// Builds an example environment containing one shell and one technical data submodel.
    /// </summary>
    /// <returns>A fully built environment used by integration tests and documentation validation.</returns>
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
