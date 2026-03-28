using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAAS.Templates.TechnicalData;
using Shouldly;

namespace FluentAAS.Templates.Tests.TechnicalData;

public class TechnicalDataBuilderTests
{
    [Fact]
    public void BuildTechnicalData_ShouldFail_WhenMandatoryParametersAreMissing()
    {
        var envBuilder = AasBuilder.Create();
        var shellBuilder = envBuilder.AddShell("urn:aas:example:shell:technical-data", "DriveShell");

        var sut = shellBuilder.AddTechnicalData("urn:aas:example:submodel:technical-data")
                              .WithMotorPerformance(motor => motor
                                  .WithRatedVoltage(400m)
                                  .WithRatedCurrent(82.5m));

        var ex = Record.Exception(() => sut.BuildTechnicalData());

        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldContain("MotorPerformance.RatedPower");
        ex.Message.ShouldContain("MotorPerformance.RatedSpeed");
        ex.Message.ShouldContain("BearingCharacteristics.InnerDiameter");
    }

    [Fact]
    public void BuildTechnicalData_ShouldFail_WhenUnitIsInvalid()
    {
        var envBuilder = AasBuilder.Create();
        var shellBuilder = envBuilder.AddShell("urn:aas:example:shell:technical-data", "DriveShell");

        var ex = Record.Exception(() => shellBuilder.AddTechnicalData("urn:aas:example:submodel:technical-data")
            .WithMotorPerformance(motor => motor.WithRatedVoltage(400m, "mm")));

        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldContain("Unit 'mm' is invalid for 'RatedVoltage'");
    }

    [Fact]
    public void BuildTechnicalData_ShouldFail_WhenSemanticIdDoesNotMatchEclassCatalogEntry()
    {
        var envBuilder = AasBuilder.Create();
        var shellBuilder = envBuilder.AddShell("urn:aas:example:shell:technical-data", "DriveShell");

        var ex = Record.Exception(() => shellBuilder.AddTechnicalData("urn:aas:example:submodel:technical-data")
            .WithMotorPerformance(motor => motor.WithRatedVoltage(400m, "V", TechnicalDataSemantics.RatedCurrent)));

        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldContain("Semantic ID mismatch for 'RatedVoltage'");
    }

    [Fact]
    public void BuildTechnicalData_ShouldCreateStructuredGroups_WithEclassSemanticIds()
    {
        var envBuilder = AasBuilder.Create();
        var shellBuilder = envBuilder.AddShell("urn:aas:example:shell:technical-data", "DriveShell");

        shellBuilder.AddTechnicalData("urn:aas:example:submodel:technical-data")
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

        var env = envBuilder.Build();

        var submodel = env.Submodels!.Single(sm => sm.Id == "urn:aas:example:submodel:technical-data");
        submodel.SemanticId!.Keys.Single().Value.ShouldBe(TechnicalDataSemantics.SubmodelTechnicalData);

        var motorGroup = submodel.SubmodelElements!
                                 .OfType<SubmodelElementCollection>()
                                 .Single(x => x.IdShort == TechnicalDataIdentifiers.MotorPerformance);

        var ratedVoltage = motorGroup.Value!
                                    .OfType<Property>()
                                    .Single(x => x.IdShort == TechnicalDataIdentifiers.RatedVoltage);

        ratedVoltage.Value.ShouldBe("400");
        ratedVoltage.Category.ShouldBe("V");
        ratedVoltage.SemanticId!.Keys.Single().Value.ShouldBe(TechnicalDataSemantics.RatedVoltage);
    }

    [Fact]
    public void CompositionExample_ShouldBuildEnvironment()
    {
        var env = TechnicalDataCompositionExample.BuildMotorAndBearingExampleEnvironment();

        env.AssetAdministrationShells.ShouldNotBeNull();
        env.AssetAdministrationShells!.Count.ShouldBe(1);
        env.Submodels.ShouldNotBeNull();
        env.Submodels!.Count.ShouldBe(1);
    }
}
