using AasCore.Aas3_0;
using AutoFixture;
using Shouldly;

namespace FluentAAS.Builder.Tests;

public class AasBuilderTests
{
    private readonly Fixture _fixture = new();

    private static AasBuilder CreateSut() => new();

    [Fact]
    public void Constructor_ShouldInitializeEmptyEnvironmentState()
    {
        // Act
        var builder = new AasBuilder();
        var environment = builder.Build();

        // Assert
        environment.ShouldNotBeNull();
        environment.AssetAdministrationShells.ShouldNotBeNull();
        environment.Submodels.ShouldNotBeNull();

        environment.AssetAdministrationShells.ShouldBeEmpty();
        environment.Submodels.ShouldBeEmpty();
    }

    [Fact]
    public void Create_ShouldReturnNewInstance()
    {
        // Arrange
        var initialBuilder = CreateSut();

        // Act
        var created = AasBuilder.Create();

        // Assert
        created.ShouldNotBeNull();
        created.ShouldBeOfType<AasBuilder>();
        created.ShouldNotBeSameAs(initialBuilder);
    }

    [Fact]
    public void AddShell_WithNullId_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = CreateSut();
        string? id = null;
        var idShort = _fixture.Create<string>();

        // Act
        var act = () => builder.AddShell(id!, idShort!);

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.ParamName.ShouldBe("id");
    }

    [Fact]
    public void AddShell_WithNullIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = CreateSut();
        var id = _fixture.Create<string>();
        string? idShort = null;

        // Act
        var act = () => builder.AddShell(id!, idShort!);

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.ParamName.ShouldBe("idShort");
    }

    [Fact]
    public void AddShell_WithValidArguments_ShouldCreateShellAndReturnShellBuilder()
    {
        // Arrange
        var builder = CreateSut();
        var id = _fixture.Create<string>();
        var idShort = _fixture.Create<string>();

        // Act
        var shellBuilder = builder.AddShell(id!, idShort!);

        // Assert
        shellBuilder.ShouldNotBeNull();
        shellBuilder.ShouldBeOfType<ShellBuilder>();

        // Ensure shell got added to the environment
        var environment = builder.Build();
        environment.AssetAdministrationShells.ShouldHaveSingleItem();

        var shell = environment.AssetAdministrationShells.Single();
        shell.Id.ShouldBe(id);
        shell.IdShort.ShouldBe(idShort);
    }

    [Fact]
    public void AddShell_WithExplicitKind_ShouldAssignKindOnShell()
    {
        // Arrange
        var builder = CreateSut();
        var id = _fixture.Create<string>();
        var idShort = _fixture.Create<string>();
        const AssetKind expectedKind = AssetKind.Type;

        // Act
        _ = builder.AddShell(id!, idShort!, expectedKind);
        var environment = builder.Build();

        // Assert
        var shell = environment.AssetAdministrationShells.ShouldHaveSingleItem();
        shell.AssetInformation.AssetKind.ShouldBe(expectedKind);
    }

    [Fact]
    public void AddExistingSubmodel_WithNullSubmodel_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = CreateSut();
        Submodel? submodel = null;

        // Act
        var act = () => builder.AddExistingSubmodel(submodel!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("submodel");
    }

    [Fact]
    public void AddExistingSubmodel_WithValidSubmodel_ShouldAddToEnvironmentAndReturnBuilder()
    {
        // Arrange
        var builder = CreateSut();
        var submodel = new Submodel(
            id: _fixture.Create<string>()!,
            idShort: _fixture.Create<string>()
        );

        // Act
        var returned = builder.AddExistingSubmodel(submodel);

        // Assert
        returned.ShouldBeSameAs(builder);

        var environment = builder.Build();
        environment.Submodels.ShouldHaveSingleItem();
        environment.Submodels.Single().ShouldBeSameAs(submodel);
    }

    [Fact]
    public void AddSubmodel_WithValidSubmodel_ShouldAddToEnvironmentAndReturnBuilder()
    {
        // Arrange
        var builder = CreateSut();
        var submodel = new Submodel(
            id: _fixture.Create<string>()!,
            idShort: _fixture.Create<string>()!
        );

        // Act
        var returned = builder.AddSubmodel(submodel);

        // Assert
        returned.ShouldBeSameAs(builder);

        var environment = builder.Build();
        environment.Submodels.ShouldNotBeNull();
        environment.Submodels.ShouldContain(submodel);
    }

    [Fact]
    public void AddSubmodel_WithInvalidIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = CreateSut();
        var submodel = new Submodel(id: _fixture.Create<string>()!)
        {
            IdShort = "  "
        };

        // Act
        var act = () => builder.AddSubmodel(submodel);

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.ParamName.ShouldBe("submodel");
    }

    [Fact]
    public void Build_WithDuplicateSubmodelIds_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = CreateSut();
        var duplicateId = _fixture.Create<string>();

        builder.AddSubmodel(new Submodel(id: duplicateId, idShort: "submodel-a"));
        builder.AddSubmodel(new Submodel(id: duplicateId, idShort: "submodel-b"));

        // Act
        var act = () => builder.Build();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Duplicate submodel id(s) detected at build-time");
    }

    [Fact]
    public void AddSubmodelFragment_ShouldApplyContributionAtBuildTime()
    {
        // Arrange
        var builder = CreateSut();
        var submodelId = _fixture.Create<string>();
        builder.AddSubmodel(new Submodel(id: submodelId, idShort: "composed-submodel"));

        // Act
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("temperature", "21.5"));
        var env = builder.Build();

        // Assert
        var submodel = env.Submodels.ShouldHaveSingleItem().ShouldBeOfType<Submodel>();
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.ShouldContain(e => e.IdShort == "temperature");
    }

    [Fact]
    public void Build_WhenCalledMultipleTimes_WithFragments_ShouldNotDuplicateFragmentElements()
    {
        // Arrange
        var builder = CreateSut();
        var submodelId = _fixture.Create<string>();
        builder.AddSubmodel(new Submodel(id: submodelId, idShort: "composed-submodel"));
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("temperature", "21.5"));

        // Act
        var env1 = builder.Build();
        var env2 = builder.Build();

        // Assert
        var firstSubmodel = env1.Submodels.ShouldHaveSingleItem().ShouldBeOfType<Submodel>();
        var secondSubmodel = env2.Submodels.ShouldHaveSingleItem().ShouldBeOfType<Submodel>();

        firstSubmodel.SubmodelElements.ShouldNotBeNull();
        secondSubmodel.SubmodelElements.ShouldNotBeNull();

        firstSubmodel.SubmodelElements.Count(e => e.IdShort == "temperature").ShouldBe(1);
        secondSubmodel.SubmodelElements.Count(e => e.IdShort == "temperature").ShouldBe(1);
    }

    [Fact]
    public void AddSubmodelFragment_WithoutBaseSubmodel_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = CreateSut();
        var missingId = _fixture.Create<string>();
        builder.AddSubmodelFragment(missingId, fragment => fragment.AddProperty("p", "v"));

        // Act
        var act = () => builder.Build();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("No base submodel");
    }

    [Fact]
    public void Build_WithShellReferencingKnownSubmodelId_ShouldSucceed()
    {
        // Arrange
        var builder = CreateSut();
        var submodelId = _fixture.Create<string>()!;
        builder.AddSubmodel(new Submodel(id: submodelId, idShort: "known-submodel"));

        var shell = new AssetAdministrationShell(
            id: _fixture.Create<string>()!,
            assetInformation: new AssetInformation(AssetKind.Instance))
        {
            IdShort = "shell-with-known-ref",
            Submodels =
            [
                new Reference(
                    ReferenceTypes.ModelReference,
                    [new Key(KeyTypes.Submodel, submodelId)])
            ]
        };

        builder.AddShellInternal(shell);

        // Act
        var environment = builder.Build();

        // Assert
        var builtShell = environment.AssetAdministrationShells!.OfType<AssetAdministrationShell>().Single();
        builtShell.Submodels.ShouldNotBeNull();
        builtShell.Submodels.Count.ShouldBe(1);
        builtShell.Submodels.Single().Keys.Single().Value.ShouldBe(submodelId);
    }

    [Fact]
    public void Build_WithShellReferencingUnknownSubmodelId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = CreateSut();
        var unknownSubmodelId = _fixture.Create<string>()!;

        var shell = new AssetAdministrationShell(
            id: _fixture.Create<string>()!,
            assetInformation: new AssetInformation(AssetKind.Instance))
        {
            IdShort = "shell-with-unknown-ref",
            Submodels =
            [
                new Reference(
                    ReferenceTypes.ModelReference,
                    [new Key(KeyTypes.Submodel, unknownSubmodelId)])
            ]
        };

        builder.AddShellInternal(shell);

        // Act
        var act = () => builder.Build();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain($"references unknown submodel id '{unknownSubmodelId}'");
    }

    [Fact]
    public void Build_WhenFragmentBatchFails_ShouldRollbackAppliedFragmentsBeforeRetry()
    {
        // Arrange
        var builder = CreateSut();
        var existingSubmodelId = _fixture.Create<string>();
        var missingSubmodelId  = _fixture.Create<string>();

        builder.AddSubmodel(new Submodel(id: existingSubmodelId, idShort: "existing"));
        builder.AddSubmodelFragment(existingSubmodelId, fragment => fragment.AddProperty("temperature", "21.5"));
        builder.AddSubmodelFragment(missingSubmodelId, fragment => fragment.AddProperty("pressure", "1.0"));

        // Act
        Should.Throw<InvalidOperationException>(() => builder.Build());

        builder.AddSubmodel(new Submodel(id: missingSubmodelId, idShort: "missing-now-added"));
        var env = builder.Build();

        // Assert
        var existingSubmodel = env.Submodels!.OfType<Submodel>().Single(s => s.Id == existingSubmodelId);
        existingSubmodel.SubmodelElements.ShouldNotBeNull();
        existingSubmodel.SubmodelElements.Count(e => e.IdShort == "temperature").ShouldBe(1);
    }

    [Fact]
    public void Build_WhenCalledMultipleTimes_ShouldReturnNewEnvironmentInstancesWithSameContent()
    {
        // Arrange
        var builder = CreateSut();

        builder.AddShell(_fixture.Create<string>()!, _fixture.Create<string>()!);

        var submodel = new Submodel(
            id: _fixture.Create<string>()!,
            idShort: _fixture.Create<string>()!
        );
        builder.AddExistingSubmodel(submodel);

        // Act
        var env1 = builder.Build();
        var env2 = builder.Build();

        // Assert
        env1.ShouldNotBeNull();
        env2.ShouldNotBeNull();
        env1.ShouldNotBeSameAs(env2); // defensively verify new instances

        env1.AssetAdministrationShells!.Count.ShouldBe(env2.AssetAdministrationShells!.Count);
        env1.Submodels!.Count.ShouldBe(env2.Submodels!.Count);
    }

    [Fact]
    public void AddShellInternal_WithNullShell_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = CreateSut();
        AssetAdministrationShell? shell = null;

        // Act
        var act = () => builder.AddShellInternal(shell!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("shell");
    }

    [Fact]
    public void AddShellInternal_WithValidShell_ShouldAddShellToEnvironment()
    {
        // Arrange
        var builder = CreateSut();

        var assetInformation = new AssetInformation(
            assetKind: AssetKind.Instance
        );

        var shell = new AssetAdministrationShell(
            id: _fixture.Create<string>()!,
            assetInformation: assetInformation,
            idShort: _fixture.Create<string>()
        );

        // Act
        builder.AddShellInternal(shell);
        var environment = builder.Build();

        // Assert
        environment.AssetAdministrationShells.ShouldHaveSingleItem();
        environment.AssetAdministrationShells.Single().ShouldBeSameAs(shell);
    }

    [Fact]
    public void AddSubmodelInternal_WithNullSubmodel_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = CreateSut();
        Submodel? submodel = null;

        // Act
        var act = () => builder.AddSubmodelInternal(submodel!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("submodel");
    }

    [Fact]
    public void AddSubmodelInternal_WithValidSubmodel_ShouldAddSubmodelToEnvironment()
    {
        // Arrange
        var builder = CreateSut();
        var submodel = new Submodel(
            id: _fixture.Create<string>()!,
            idShort: _fixture.Create<string>()!
        );

        // Act
        builder.AddSubmodelInternal(submodel);
        var environment = builder.Build();

        // Assert
        environment.Submodels.ShouldHaveSingleItem();
        environment.Submodels.Single().ShouldBeSameAs(submodel);
    }

    [Fact]
    public void AddSubmodelInternal_WithMissingIdShort_ShouldStillAddSubmodel()
    {
        // Arrange
        var builder = CreateSut();
        var submodel = new Submodel(id: _fixture.Create<string>()!);

        // Act
        builder.AddSubmodelInternal(submodel);
        var environment = builder.Build();

        // Assert
        environment.Submodels.ShouldHaveSingleItem();
        environment.Submodels.Single().ShouldBeSameAs(submodel);
    }
}
