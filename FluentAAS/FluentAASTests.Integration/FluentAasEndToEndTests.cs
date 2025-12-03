using AasCore.Aas3_0;
using FluentAas.Builder;
using FluentAas.Core;
using FluentAas.IO;
using FluentAas.Validation;
using FluentAas.Validation.Rules;
using Shouldly;

namespace FluentAasTests.Integration;

public class FluentAasEndToEndTests
{
    [Fact]
    public void Build_Validate_Serialize_And_Deserialize_Environment()
    {
        // 1) Build a submodel
        var submodel = SubmodelBuilder.Create("urn:submodel:demo")
                                      .AddProperty("ManufacturerName", "ACME Corp")
                                      .AddProperty("ModelNumber", "X-123")
                                      .Build();

        // 2) Build AAS with asset info and submodel reference
        var assetInfo = new AssetInformation(
                                             assetKind: AssetKind.Instance,
                                             globalAssetId: "urn:asset:123"
                                            );

        var submodelRef = new Reference(
                                        type: ReferenceTypes.ModelReference,
                                        keys: new List<IKey>(
                                                             new List<Key>
                                                             {
                                                                 new(
                                                                     type: KeyTypes.Submodel,
                                                                     value: submodel.Id
                                                                    )
                                                             })
                                       );

        var shell = AssetAdministrationShellBuilder
                    .Create("urn:aas:demo")
                    .WithAssetInformation(assetInfo)
                    .AddSubmodelReference(submodelRef)
                    .Build();

        // 3) Build environment via fluent DSL
        IAasEnvironmentAdapter envAdapter = AasFluent.CreateEnvironment()
                                                     .AddShell(shell)
                                                     .AddSubmodel(submodel)
                                                     .Build();

        // 4) Validate
        var validationService = new ValidationService(
                                                      new IValidationRule[]
                                                      {
                                                          new NonEmptyIdRule(),
                                                          new UniqueIdRule()
                                                      });

        ValidationReport report = validationService.Validate(envAdapter);

        // ASSERT: validation succeeded
        report.ShouldNotBeNull();
        report.HasErrors.ShouldBeFalse();
        report.Results.ShouldAllBe(r => r.Level != ValidationLevel.Error);

        // 5) Serialize
        string json = AasJsonSerializer.ToJson(envAdapter);

        // ASSERT: JSON looks sane
        json.ShouldNotBeNullOrWhiteSpace();
        json.ShouldContain("urn:aas:demo");
        json.ShouldContain("urn:submodel:demo");
        json.ShouldContain("ACME Corp");

        // 6) Deserialize back
        IAasEnvironmentAdapter deserialized = AasJsonSerializer.FromJson(json);

        // ASSERT: roundtrip preserved core structure
        deserialized.ShouldNotBeNull();
        deserialized.Environment.ShouldNotBeNull();

        var shells = deserialized.Environment.AssetAdministrationShells;
        shells.ShouldNotBeNull();
        shells!.Count.ShouldBe(1);
        shells.Single().Id.ShouldBe(shell.Id);

        var submodels = deserialized.Environment.Submodels;
        submodels.ShouldNotBeNull();
        submodels!.Count.ShouldBe(1);

        var roundtrippedSubmodel = submodels.Single();
        roundtrippedSubmodel.Id.ShouldBe(submodel.Id);
        roundtrippedSubmodel.SubmodelElements.ShouldNotBeNull();
        roundtrippedSubmodel.SubmodelElements!.Count.ShouldBe(2);

        var manufacturerProp = roundtrippedSubmodel.SubmodelElements
                                                   .OfType<Property>()
                                                   .Single(p => p.IdShort == "ManufacturerName");
        manufacturerProp.Value.ShouldBe("ACME Corp");

        var modelNumberProp = roundtrippedSubmodel.SubmodelElements
                                                  .OfType<Property>()
                                                  .Single(p => p.IdShort == "ModelNumber");
        modelNumberProp.Value.ShouldBe("X-123");
    }
}