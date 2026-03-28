using FluentAAS.Builder;

namespace FluentAAS.Templates.TechnicalData;

public static class TechnicalDataCompositionExample
{
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
