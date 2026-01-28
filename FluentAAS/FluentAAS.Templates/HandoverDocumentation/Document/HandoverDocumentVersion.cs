using System.Globalization;

namespace FluentAAS.Templates.HandoverDocumentation.Document;

public class HandoverDocumentVersion
{
    // Required
    public List<string> Languages                { get; }      = new(); // SML of Properties (string)
    public string       Version                  { get; set; } = "1.0";
    public string       Title                    { get; set; } = string.Empty;
    public string       TitleLanguage            { get; set; } = "en";
    public string       Description              { get; set; } = string.Empty;
    public string       DescriptionLanguage      { get; set; } = "en";
    public DateTime     StatusSetDate            { get; set; } = DateTime.UtcNow.Date;
    public string       StatusValue              { get; set; } = HandoverDocumentationSemantics.StatusValues.Released;
    public string       OrganizationShortName    { get; set; } = string.Empty;
    public string       OrganizationOfficialName { get; set; } = string.Empty;

    // Optional
    public string?                                 Subtitle         { get; set; }
    public string?                                 SubtitleLanguage { get; set; }
    public List<(string Language, string Keyword)> KeyWords         { get; } = [];

    // Files (required list)
    public List<HandoverDigitalFile> DigitalFiles { get; } = [];

    // Optional
    public HandoverDigitalFile? PreviewFile { get; set; }

    internal SubmodelElementCollection ToCollection()
    {
        var elements = new List<ISubmodelElement>
                       {
                           ToLanguageList(),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortVersion,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdVersion),
                                        valueType: DataTypeDefXsd.String,
                                        value: Version
                                       ),
                           new MultiLanguageProperty(
                                                     idShort: HandoverDocumentationSemantics.IdShortTitle,
                                                     category: null,
                                                     semanticId: Ref(HandoverDocumentationSemantics.SemanticIdTitle),
                                                     value: new List<ILangStringTextType>([new LangStringTextType(TitleLanguage, Title)])
                                                    ),
                           new MultiLanguageProperty(
                                                     idShort: HandoverDocumentationSemantics.IdShortDescription,
                                                     category: null,
                                                     semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDescription),
                                                     value: new List<ILangStringTextType>([new LangStringTextType(DescriptionLanguage, Description)])
                                                    ),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortStatusSetDate,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdStatusSetDate),
                                        valueType: DataTypeDefXsd.Date,
                                        value: StatusSetDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                                       ),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortStatusValue,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdStatusValue),
                                        valueType: DataTypeDefXsd.String,
                                        value: StatusValue
                                       ),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortOrganizationShortName,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdOrganizationShortName),
                                        valueType: DataTypeDefXsd.String,
                                        value: OrganizationShortName
                                       ),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortOrganizationOfficialName,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdOrganizationOfficialName),
                                        valueType: DataTypeDefXsd.String,
                                        value: OrganizationOfficialName
                                       ),
                           ToDigitalFilesList()
                       };

        if (!string.IsNullOrWhiteSpace(Subtitle))
        {
            elements.Add(
                         new MultiLanguageProperty(
                                                   idShort: HandoverDocumentationSemantics.IdShortSubtitle,
                                                   category: null,
                                                   semanticId: Ref(HandoverDocumentationSemantics.SemanticIdSubtitle),
                                                   value: [new LangStringTextType(SubtitleLanguage ?? TitleLanguage, Subtitle!)]
                                                  )
                        );
        }

        if (KeyWords.Count > 0)
        {
            // Template defines KeyWords as MLP (0..1). We store as multiple LangString entries.
            elements.Add(
                         new MultiLanguageProperty(
                                                   idShort: HandoverDocumentationSemantics.IdShortKeyWords,
                                                   category: null,
                                                   semanticId: Ref(HandoverDocumentationSemantics.SemanticIdKeyWords),
                                                   value: [..KeyWords.Select(k => new LangStringTextType(k.Language, k.Keyword)).ToArray()]
                                                  )
                        );
        }

        if (PreviewFile is not null)
        {
            elements.Add(
                         PreviewFile.ToFileElement(
                                                   idShort: HandoverDocumentationSemantics.IdShortPreviewFile,
                                                   semanticId: HandoverDocumentationSemantics.SemanticIdPreviewFile));
        }

        return new SubmodelElementCollection(
                                             idShort: "DocumentVersion",
                                             category: null,
                                             description: null,
                                             semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocumentVersion),
                                             value: [..elements.ToArray()]
                                            );
    }

    internal void ValidateTemplateRequirements()
    {
        if (Languages.Count < 1)
            throw new InvalidOperationException("DocumentVersion must contain at least one Language.");

        if (string.IsNullOrWhiteSpace(Version))
            throw new InvalidOperationException("DocumentVersion.Version is required.");

        if (string.IsNullOrWhiteSpace(Title))
            throw new InvalidOperationException("DocumentVersion.Title is required.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new InvalidOperationException("DocumentVersion.Description is required.");

        if (string.IsNullOrWhiteSpace(StatusValue))
            throw new InvalidOperationException("DocumentVersion.StatusValue is required.");

        if (string.IsNullOrWhiteSpace(OrganizationShortName))
            throw new InvalidOperationException("DocumentVersion.OrganizationShortName is required.");

        if (string.IsNullOrWhiteSpace(OrganizationOfficialName))
            throw new InvalidOperationException("DocumentVersion.OrganizationOfficialName is required.");

        if (DigitalFiles.Count < 1)
            throw new InvalidOperationException("DocumentVersion must contain at least one DigitalFile (DigitalFiles list).");

        foreach (var f in DigitalFiles)
            f.Validate();
        PreviewFile?.Validate();
    }

    private SubmodelElementList ToLanguageList()
    {
        // SML of Properties (string)
        var langElements = Languages
                           .Select(ISubmodelElement (l) => new Property(
                                                                        idShort: "Language",
                                                                        category: null,
                                                                        semanticId: null, // list element semantic handled by listElementSemanticId
                                                                        valueType: DataTypeDefXsd.String,
                                                                        value: l))
                           .ToList();

        var list = new SubmodelElementList(
                                           typeValueListElement: AasSubmodelElements.SubmodelElement,
                                           idShort: HandoverDocumentationSemantics.IdShortLanguage,
                                           category: null,
                                           description: null,
                                           semanticId: Ref(HandoverDocumentationSemantics.SemanticIdLanguage),
                                           value: [..langElements.ToArray()]
                                          );

        list.OrderRelevant = false;
        // For list-of-primitive-properties, some SDKs allow semanticIdListElement. If yours requires it, set it.
        // list.SemanticIdListElement = ToSemanticReference(...); // Not specified as a separate semanticId in the PDF; keep null.
        list.TypeValueListElement = AasSubmodelElements.Property;

        return list;
    }

    private SubmodelElementList ToDigitalFilesList()
    {
        var fileElements = DigitalFiles
                           .Select(ISubmodelElement (f) => f.ToFileElement(
                                                                           idShort: HandoverDocumentationSemantics.IdShortDigitalFiles + "_File",
                                                                           semanticId: null // list handles semanticIdListElement if your SDK supports it
                                                                          ))
                           .ToList();

        var list = new SubmodelElementList(
                                           typeValueListElement: AasSubmodelElements.SubmodelElement,
                                           idShort: HandoverDocumentationSemantics.IdShortDigitalFiles,
                                           category: null,
                                           description: null,
                                           semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDigitalFiles),
                                           value: [..fileElements.ToArray()]
                                          );

        list.OrderRelevant = false;
        // PDF defines DigitalFiles as SML; element semantic id not called out separately in Table 8,
        // so we do not invent one. Keep null unless your validators require a list element semantic.
        // list.SemanticIdListElement = ...;
        list.TypeValueListElement = AasSubmodelElements.File;

        return list;
    }

    private static Reference Ref(string semanticId) =>
        new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
}