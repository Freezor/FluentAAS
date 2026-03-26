using AasCore.Aas3_0;
using FluentAAS.Templates.DigitalNameplate;
using Shouldly;
using static FluentAAS.Templates.DigitalNameplate.DigitalNameplateSemantics;

namespace FluentAAS.Templates.Tests.DigitalNameplate;

public class ContactInformationBuilderTests
{
    [Fact]
    public void Build_ShouldCreateStructuredContactInformation_WhenAllFieldsAreValid()
    {
        var sut = new ContactInformationBuilder()
            .WithManufacturerContact("ACME Service", "+49 89 123456")
            .WithServiceHotline("+49 800 123456")
            .WithEmail("service@acme.example")
            .WithWebsiteUrl("https://acme.example/support")
            .WithAddress("Main Street 1", "Munich", "80331", "DE", "Bavaria");

        var contact = sut.Build();

        contact.IdShort.ShouldBe(DigitalNameplateIdentifiers.ContactInformationIdShort);
        contact.SemanticId!.Keys.Single().Value.ShouldBe(ContactInformation);

        var manufacturer = contact.Value!
                                  .OfType<SubmodelElementCollection>()
                                  .Single(x => x.IdShort == DigitalNameplateIdentifiers.ManufacturerContactIdShort);
        manufacturer.SemanticId!.Keys.Single().Value.ShouldBe(ManufacturerContact);

        var hotline = contact.Value!
                             .OfType<Property>()
                             .Single(x => x.IdShort == DigitalNameplateIdentifiers.ServiceHotlineIdShort);
        hotline.Value.ShouldBe("+49 800 123456");

        var address = contact.Value!
                             .OfType<SubmodelElementCollection>()
                             .Single(x => x.IdShort == DigitalNameplateIdentifiers.AddressInformationIdShort);
        address.SemanticId!.Keys.Single().Value.ShouldBe(ContactAddressInformation);
    }

    [Fact]
    public void Build_ShouldThrow_WhenManufacturerRoleIsMissing()
    {
        var sut = new ContactInformationBuilder()
            .WithServiceHotline("+49 800 123456");

        var ex = Should.Throw<InvalidOperationException>(() => sut.Build());
        ex.Message.ShouldContain("ManufacturerContact");
    }

    [Fact]
    public void Build_ShouldThrow_WhenServiceHotlineRoleIsMissing()
    {
        var sut = new ContactInformationBuilder()
            .WithManufacturerContact("ACME Service");

        var ex = Should.Throw<InvalidOperationException>(() => sut.Build());
        ex.Message.ShouldContain("ServiceHotline");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("x@")]
    public void WithEmail_ShouldThrow_WhenEmailIsInvalid(string email)
    {
        var sut = new ContactInformationBuilder();

        var ex = Should.Throw<ArgumentException>(() => sut.WithEmail(email));
        ex.Message.ShouldContain("Email format is invalid");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp//broken")]
    public void WithWebsiteUrl_ShouldThrow_WhenUrlIsInvalid(string url)
    {
        var sut = new ContactInformationBuilder();

        var ex = Should.Throw<ArgumentException>(() => sut.WithWebsiteUrl(url));
        ex.Message.ShouldContain("URL format is invalid");
    }
}
