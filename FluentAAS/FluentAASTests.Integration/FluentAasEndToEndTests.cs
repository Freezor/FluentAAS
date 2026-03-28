#region

    using AasCore.Aas3_0;
    using FluentAAS.Builder;
    using FluentAas.IO;
    using FluentAAS.IO;
    using FluentAAS.Templates.DigitalNameplate;
    using FluentAAS.Templates.HandoverDocumentation;
    using FluentAAS.Templates.HandoverDocumentation.Document;
    using FluentAAS.Templates.TechnicalData;
    using Shouldly;
    using File = System.IO.File;

#endregion


    namespace FluentAASTests.Integration;

    /// <summary>
    ///     Contains integration tests for building a Digital Nameplate submodel
    ///     using the fluent FluentAAS builder API.
    /// </summary>
    public class FluentAasEndToEndTests
    {
        /// <summary>
        ///     Verifies that a Digital Nameplate submodel can be created and attached
        ///     to an Asset Administration Shell using the fluent API.
        ///     Every piece of content configured via the builder is asserted.
        /// </summary>
        [Fact]
        public void CanCreateDigitalNameplateWithFluentApi()
        {
            // Arrange & Act
            var environment = AasBuilder.Create()
                                        .AddShell("urn:aas:example:my-shell", "MyShell")
                                        .WithGlobalAssetId("urn:asset:example:my-asset")
                                        .AddSubmodel("urn:aas:example:generic-submodel:1", "GenericSubmodel")
                                        .WithSemanticId(
                                                        new Reference(
                                                                      ReferenceTypes.ExternalReference,
                                                                      [
                                                                          new Key(
                                                                                  KeyTypes.Submodel,
                                                                                  "urn:aas:example:generic-submodel:semantic-id")
                                                                      ]
                                                                     ))
                                        .AddMultiLanguageProperty(
                                                                  "GenericMultiLanguageProperty", ls => ls
                                                                                                        .Add("en", "Example value")
                                                                                                        .Add("de", "Beispielwert"))
                                        .AddElement("GenericProperty", "example value")
                                        .CompleteSubmodelConfiguration()
                                        .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
                                        .WithManufacturerName("de", "Muster AG")
                                        .WithManufacturerName("en", "Sample Corp")
                                        .WithManufacturerProductDesignation("de", "Super-Antriebseinheit XS")
                                        .WithManufacturerProductDesignation("en", "Super Drive Unit XS")
                                        .WithSerialNumber("SN-000123")
                                        .BuildDigitalNameplate()
                                        .CompleteShellConfiguration()
                                        .Build();

            // Create De/Serializing environment
            var json               = AasJsonSerializer.ToJson(environment);
            var createdEnvironment = AasJsonSerializer.FromJson(json);

            // Create AASX package
            const string aasxPath = "./test.aasx";
            environment.ToAasx(aasxPath, "/spec/uri/env.json");
            var extractedAasEnvironment = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

            File.Delete(aasxPath);

            // Assert: environment basics
            environment.ShouldNotBeNull();
            environment.AssetAdministrationShells.ShouldHaveSingleItem();
            environment.Submodels!.Count.ShouldBe(2);

            // Assert: shell and asset info
            var shell = environment.AssetAdministrationShells!.First();
            shell.IdShort.ShouldBe("MyShell");
            shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");

            // Assert: submodel basics
            var submodel = environment.Submodels!.Find(x => x.IdShort!.Equals("DigitalNameplate"));
            submodel.ShouldNotBeNull();
            submodel.SubmodelElements!.Count.ShouldBe(3);

            // Assert: Digital Nameplate content
            var manuName = GetMultiLanguageProperty(submodel, "ManufacturerName");
            manuName.ShouldNotBeNull("ManufacturerName should be present");
            manuName.Value!.First(l => l.Language == "de").Text
                    .ShouldBe("Muster AG");
            manuName.Value!.First(l => l.Language == "en").Text
                    .ShouldBe("Sample Corp");

            // Manufacturer product designation (multi-language)
            var manuProdDesig = GetMultiLanguageProperty(submodel, "ManufacturerProductDesignation");
            manuProdDesig.ShouldNotBeNull("ManufacturerProductDesignation should be present");
            manuProdDesig.Value!.First(l => l.Language == "de").Text
                         .ShouldBe("Super-Antriebseinheit XS");
            manuProdDesig.Value!.First(l => l.Language == "en").Text
                         .ShouldBe("Super Drive Unit XS");

            // Serial number (string property)
            var serialNumber = GetProperty(submodel, "SerialNumber");
            serialNumber.ShouldNotBeNull("SerialNumber should be present");
            serialNumber.Value.ShouldBe("SN-000123");

            // Assert: round-trip JSON de/serialization
            createdEnvironment.ShouldBeEquivalentTo(
                                                    environment,
                                                    "Serializing and Deserializing the same object should result in identical objects");

            // Assert: round-trip AASX de/serialization
            extractedAasEnvironment.ShouldBeEquivalentTo(environment, "Packaging to .aasx and reading from the same object should result in identical objects");
        }

        /// <summary>
        ///     Verifies that the <see cref="DigitalNameplateBuilder" /> throws an
        ///     <see cref="InvalidOperationException" /> when required fields are missing.
        /// </summary>
        [Fact]
        public void DigitalNameplateBuilder_ThrowsOnMissingRequiredFields()
        {
            // Arrange
            var builder = AasBuilder.Create()
                                    .AddShell("urn:aas:example:my-shell", "MyShell")
                                    .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
                                    .WithManufacturerName("de", "Muster AG");

            // Act
            var exception = Should.Throw<InvalidOperationException>(() => builder.BuildDigitalNameplate());

            // Assert
            exception.Message.ShouldContain("Digital Nameplate");
            exception.Message.ShouldContain("ManufacturerProductDesignation");
            exception.Message.ShouldContain("SerialNumber");
        }

        /// <summary>
        ///     Verifies that a Handover Documentation submodel can be created and attached
        ///     to an Asset Administration Shell using the fluent API.
        /// </summary>
        [Fact]
        public void CanCreateHandoverDocumentationWithFluentApi()
        {
            // Arrange & Act
            var testDate    = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);
            var environment = CreateHandoverDocumentationEnvironment(testDate);

            // Assert
            AssertEnvironmentStructure(environment);
            AssertShellProperties(environment);
            AssertSubmodelProperties(environment);
            AssertDocumentContent(environment);
        }

        /// <summary>
        ///     Verifies that the Technical Data template can be composed as part of a full environment
        ///     and persists structured motor and bearing parameters with expected ECLASS semantics.
        /// </summary>
        [Fact]
        public void CanCreateTechnicalDataWithFluentApi()
        {
            var environment = TechnicalDataCompositionExample.BuildMotorAndBearingExampleEnvironment();

            environment.AssetAdministrationShells.ShouldNotBeNull();
            environment.AssetAdministrationShells!.Count.ShouldBe(1);
            environment.Submodels.ShouldNotBeNull();
            environment.Submodels!.Count.ShouldBe(1);

            var technicalData = environment.Submodels!.Single(sm => sm.Id == "urn:aas:submodel:technical-data:drivetrain:1000");
            technicalData.SemanticId!.Keys.Single().Value.ShouldBe(TechnicalDataSemantics.SubmodelTechnicalData);

            var motorPerformance = technicalData.SubmodelElements!
                                               .OfType<SubmodelElementCollection>()
                                               .Single(x => x.IdShort == TechnicalDataIdentifiers.MotorPerformance);
            var ratedVoltage = motorPerformance.Value!
                                               .OfType<Property>()
                                               .Single(x => x.IdShort == TechnicalDataIdentifiers.RatedVoltage);

            ratedVoltage.Value.ShouldBe("400");
            ratedVoltage.SemanticId!.Keys.Single().Value.ShouldBe(TechnicalDataSemantics.RatedVoltage);
            ratedVoltage.Category.ShouldBe("V");
        }

        private static IEnvironment CreateHandoverDocumentationEnvironment(DateTime testDate)
        {
            return AasBuilder.Create()
                             .AddShell("urn:aas:example:my-shell", "MyShell")
                             .WithGlobalAssetId("urn:asset:example:my-asset")
                             .AddHandoverDocumentation("urn:submodel:example:handover-documentation:V2_0")
                             .WithDescription("en", "Complete handover documentation for the asset")
                             .WithDescription("de", "Vollständige Übergabedokumentation für das Asset")
                             .WithCategory("INSTANCE")
                             .AddDocument(doc => doc
                                                 .AddDocumentId("URI", "DOC-001")
                                                 .WithDocumentClassification("01-01", "Installation Manual")
                                                 .AddDocumentVersion(ver => ver
                                                                            .WithLanguage("en")
                                                                            .WithLanguage("de")
                                                                            .WithVersion("1.0")
                                                                            .WithTitle("Installation Manual Document")
                                                                            .WithDescription("A comprehensive installation manual document")
                                                                            .WithSubtitle("Quick Start Guide")
                                                                            .AddKeyword("installation")
                                                                            .AddKeyword("manual")
                                                                            .AddKeyword("anleitung", "de")
                                                                            .WithStatus("Released", testDate)
                                                                            .WithOrganization("CRM", "Customer Relations Management")
                                                                            .AddDigitalFile("installation_manual.pdf", "application/pdf")
                                                                            .AddDigitalFile(
                                                                                            "installation_guide.docx",
                                                                                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                                                                            .WithPreviewFile("path/to/preview.pdf", "application/pdf")
                                                                    )
                                                 )
                             .BuildHandoverDocumentation()
                             .CompleteShellConfiguration()
                             .Build();
        }

        private static void AssertEnvironmentStructure(IEnvironment environment)
        {
            environment.ShouldNotBeNull();
            environment.AssetAdministrationShells.ShouldNotBeNull();
            environment.AssetAdministrationShells.Count.ShouldBe(1);
            environment.Submodels.ShouldNotBeNull();
            environment.Submodels.Count.ShouldBe(1);
        }

        private static void AssertShellProperties(IEnvironment environment)
        {
            var shell = environment.AssetAdministrationShells!.First();
            shell.Id.ShouldBe("urn:aas:example:my-shell");
            shell.IdShort.ShouldBe("MyShell");
            shell.AssetInformation.ShouldNotBeNull();
            shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");
            shell.Submodels.ShouldNotBeNull();
            shell.Submodels.Count.ShouldBe(1);
        }

        private static void AssertSubmodelProperties(IEnvironment environment)
        {
            var submodel = environment.Submodels!.First();
            submodel.Id.ShouldBe("urn:submodel:example:handover-documentation:V2_0");
            submodel.IdShort.ShouldBe("HandoverDocumentation");
            submodel.Category.ShouldBe("INSTANCE");
            submodel.SemanticId.ShouldNotBeNull();
            submodel.SemanticId.Type.ShouldBe(ReferenceTypes.ExternalReference);
            submodel.SemanticId.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SubmodelSemanticId);

            // Assert descriptions
            submodel.Description.ShouldNotBeNull();
            submodel.Description.Count.ShouldBe(2);
            submodel.Description.ShouldContain(d => d.Language == "en" && d.Text == "Complete handover documentation for the asset");
            submodel.Description.ShouldContain(d => d.Language == "de" && d.Text == "Vollständige Übergabedokumentation für das Asset");

            // Assert documents list structure
            submodel.SubmodelElements.ShouldNotBeNull();
            submodel.SubmodelElements.Count.ShouldBe(1);
        }

        private static void AssertDocumentContent(IEnvironment environment)
        {
            var submodel           = environment.Submodels!.First();
            var documentsList      = GetDocumentsList(submodel);
            var documentCollection = GetDocumentCollection(documentsList);

            AssertDocumentIds(documentCollection);
            AssertDocumentClassifications(documentCollection);
            AssertDocumentVersions(documentCollection);
        }

        private static SubmodelElementList GetDocumentsList(ISubmodel submodel)
        {
            var documentsList = submodel.SubmodelElements!.OfType<SubmodelElementList>()
                                        .Single(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocuments);
            documentsList.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocuments);
            documentsList.TypeValueListElement.ShouldBe(AasSubmodelElements.SubmodelElementCollection);
            documentsList.Value.ShouldNotBeNull();
            documentsList.Value.Count.ShouldBe(1);
            return documentsList;
        }

        private static SubmodelElementCollection GetDocumentCollection(SubmodelElementList documentsList)
        {
            var documentCollection = documentsList.Value!.OfType<SubmodelElementCollection>().Single();
            documentCollection.IdShort.ShouldBe("Document");
            documentCollection.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocument);
            return documentCollection;
        }

        private static void AssertDocumentIds(SubmodelElementCollection documentCollection)
        {
            var documentIds = GetSubmodelElementList(documentCollection, HandoverDocumentationSemantics.IdShortDocumentIds);
            documentIds.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocumentIds);
            documentIds.Value!.Count.ShouldBe(1);

            var documentIdCollection = documentIds.Value.OfType<SubmodelElementCollection>().Single();
            GetPropertyLocal(documentIdCollection, HandoverDocumentationSemantics.IdShortDocumentDomainId).Value.ShouldBe("URI");
            GetPropertyLocal(documentIdCollection, HandoverDocumentationSemantics.IdShortDocumentIdentifier).Value.ShouldBe("DOC-001");
            GetPropertyLocal(documentIdCollection, HandoverDocumentationSemantics.IdShortDocumentIsPrimary).Value.ShouldBe("true");
        }

        private static void AssertDocumentClassifications(SubmodelElementCollection documentCollection)
        {
            var documentClassifications = GetSubmodelElementList(documentCollection, HandoverDocumentationSemantics.IdShortDocumentClassifications);
            documentClassifications.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocumentClassifications);
            documentClassifications.Value!.Count.ShouldBe(1);

            var classificationCollection = documentClassifications.Value.OfType<SubmodelElementCollection>().Single();
            GetPropertyLocal(classificationCollection, HandoverDocumentationSemantics.IdShortClassId).Value.ShouldBe("01-01");
            GetMultiLanguagePropertyLocal(classificationCollection, HandoverDocumentationSemantics.IdShortClassName)
                .Value!.ShouldContain(v => v.Language == "en" && v.Text == "Installation Manual");
            GetPropertyLocal(classificationCollection, HandoverDocumentationSemantics.IdShortClassificationSystem).Value.ShouldBe("VDI 2770 Blatt 1:2020");
        }

        private static void AssertDocumentVersions(SubmodelElementCollection documentCollection)
        {
            var documentVersions = GetSubmodelElementList(documentCollection, HandoverDocumentationSemantics.IdShortDocumentVersions);
            documentVersions.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocumentVersions);
            documentVersions.Value!.Count.ShouldBe(1);

            var versionCollection = documentVersions.Value.OfType<SubmodelElementCollection>().Single();
            AssertVersionLanguages(versionCollection);
            AssertVersionProperties(versionCollection);
            AssertDigitalFiles(versionCollection);
            AssertPreviewFile(versionCollection);
        }

        private static void AssertVersionLanguages(SubmodelElementCollection versionCollection)
        {
            var languageList = GetSubmodelElementList(versionCollection, HandoverDocumentationSemantics.IdShortLanguage);
            languageList.Value!.Count.ShouldBe(2);
            var languageProperties = languageList.Value.OfType<Property>().ToList();
            languageProperties.Select(p => p.Value).ShouldContain("en");
            languageProperties.Select(p => p.Value).ShouldContain("de");
        }

        private static void AssertVersionProperties(SubmodelElementCollection versionCollection)
        {
            GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortVersion).Value.ShouldBe("1.0");

            var titleProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortTitle);
            titleProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Installation Manual Document");

            var descriptionProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortDescription);
            descriptionProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "A comprehensive installation manual document");

            var subtitleProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortSubtitle);
            subtitleProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Quick Start Guide");

            AssertKeywords(versionCollection);
            AssertStatusAndOrganization(versionCollection);
        }

        private static void AssertKeywords(SubmodelElementCollection versionCollection)
        {
            var keywordsProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortKeyWords);
            keywordsProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "installation");
            keywordsProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "manual");
            keywordsProperty.Value!.ShouldContain(v => v.Language == "de" && v.Text == "anleitung");
        }

        private static void AssertStatusAndOrganization(SubmodelElementCollection versionCollection)
        {
            GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortStatusValue).Value.ShouldBe("Released");
            GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortOrganizationShortName).Value.ShouldBe("CRM");
            GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortOrganizationOfficialName).Value.ShouldBe("Customer Relations Management");
        }

        private static void AssertDigitalFiles(SubmodelElementCollection versionCollection)
        {
            var digitalFilesList = GetSubmodelElementList(versionCollection, HandoverDocumentationSemantics.IdShortDigitalFiles);
            digitalFilesList.Value!.Count.ShouldBe(2);

            var digitalFiles = digitalFilesList.Value.OfType<AasCore.Aas3_0.File>().ToList();
            digitalFiles.ShouldContain(f => f.Value == "installation_manual.pdf" && f.ContentType == "application/pdf");
            digitalFiles.ShouldContain(f => f.Value == "installation_guide.docx" && f.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        private static void AssertPreviewFile(SubmodelElementCollection versionCollection)
        {
            var previewFile = GetFile(versionCollection, HandoverDocumentationSemantics.IdShortPreviewFile);
            previewFile.Value.ShouldBe("path/to/preview.pdf");
            previewFile.ContentType.ShouldBe("application/pdf");
            previewFile.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdPreviewFile);
        }

        private static AasCore.Aas3_0.File GetFile(SubmodelElementCollection collection, string idShort)
        {
            return collection.Value!.OfType<AasCore.Aas3_0.File>().Single(p => p.IdShort == idShort);
        }

        private static SubmodelElementList GetSubmodelElementList(SubmodelElementCollection collection, string idShort)
        {
            return collection.Value!.OfType<SubmodelElementList>().Single(p => p.IdShort == idShort);
        }

        private static MultiLanguageProperty GetMultiLanguagePropertyLocal(SubmodelElementCollection collection, string idShort)
        {
            return collection.Value!.OfType<MultiLanguageProperty>().Single(p => p.IdShort == idShort);
        }

        private static Property GetPropertyLocal(SubmodelElementCollection collection, string idShort)
        {
            return collection.Value!.OfType<Property>().Single(p => p.IdShort == idShort);
        }

        /// <summary>
        ///     Verifies that the HandoverDocumentationSubmodelBuilder throws an
        ///     InvalidOperationException when no documents are added.
        /// </summary>
        [Fact]
        public void HandoverDocumentationBuilder_ThrowsOnMissingRequiredDocuments()
        {
            // Arrange
            var builder = AasBuilder.Create()
                                    .AddShell("urn:aas:example:my-shell", "MyShell")
                                    .AddHandoverDocumentation("urn:submodel:example:handover-documentation:V2_0")
                                    .WithDescription("en", "Test documentation");

            // Act
            var exception = Should.Throw<InvalidOperationException>(() => builder.BuildHandoverDocumentation());

            // Assert
            exception.Message.ShouldContain("Handover Documentation");
            exception.Message.ShouldContain("Document");
        }

        private static Property? GetProperty(ISubmodel submodel, string idShort)
        {
            return submodel.SubmodelElements!
                           .OfType<Property>()
                           .FirstOrDefault(p => p.IdShort == idShort);
        }

        private static MultiLanguageProperty? GetMultiLanguageProperty(ISubmodel submodel, string idShort)
        {
            return submodel.SubmodelElements!
                           .OfType<MultiLanguageProperty>()
                           .FirstOrDefault(p => p.IdShort == idShort);
        }
    }
