using System.Net.Mail;
using static FluentAAS.Templates.DigitalNameplate.DigitalNameplateSemantics;

namespace FluentAAS.Templates.DigitalNameplate;

/// <summary>
/// Fluent builder for structured Digital Nameplate contact information.
/// </summary>
public sealed class ContactInformationBuilder
{
    private string?                    _manufacturerName;
    private string?                    _manufacturerPhone;
    private string?                    _serviceHotline;
    private string?                    _email;
    private string?                    _websiteUrl;
    private AddressInformationBuilder? _address;

    /// <summary>
    /// Configures the manufacturer contact role.
    /// </summary>
    /// <param name="name">Display name of the manufacturer contact.</param>
    /// <param name="phone">Optional manufacturer phone number.</param>
    /// <returns>The current <see cref="ContactInformationBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
    public ContactInformationBuilder WithManufacturerContact(string name, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Manufacturer contact name must not be empty.", nameof(name));
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            ValidatePhone(phone, nameof(phone));
        }

        _manufacturerName  = name;
        _manufacturerPhone = phone;
        return this;
    }

    /// <summary>
    /// Configures the service hotline contact role.
    /// </summary>
    /// <param name="hotline">The hotline number.</param>
    /// <returns>The current <see cref="ContactInformationBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="hotline"/> is null, empty, or whitespace.</exception>
    public ContactInformationBuilder WithServiceHotline(string hotline)
    {
        if (string.IsNullOrWhiteSpace(hotline))
        {
            throw new ArgumentException("Service hotline must not be empty.", nameof(hotline));
        }

        ValidatePhone(hotline, nameof(hotline));

        _serviceHotline = hotline;
        return this;
    }

    /// <summary>
    /// Sets the primary support email address.
    /// </summary>
    /// <param name="email">Email address.</param>
    /// <returns>The current <see cref="ContactInformationBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the email format is invalid.</exception>
    public ContactInformationBuilder WithEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email must not be empty.", nameof(email));
        }

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Email format is invalid.", nameof(email));
        }

        _email = email;
        return this;
    }

    /// <summary>
    /// Sets the primary support website URL.
    /// </summary>
    /// <param name="url">Absolute URL.</param>
    /// <returns>The current <see cref="ContactInformationBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the URL format is invalid.</exception>
    public ContactInformationBuilder WithWebsiteUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL must not be empty.", nameof(url));
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            throw new ArgumentException("URL format is invalid.", nameof(url));
        }

        _websiteUrl = url;
        return this;
    }

    /// <summary>
    /// Configures structured address information.
    /// </summary>
    /// <param name="street">Street and house number.</param>
    /// <param name="city">City.</param>
    /// <param name="postalCode">Postal/zip code.</param>
    /// <param name="countryCode">Two-letter ISO country code.</param>
    /// <param name="state">Optional state or region.</param>
    /// <returns>The current <see cref="ContactInformationBuilder"/> instance.</returns>
    public ContactInformationBuilder WithAddress(
        string  street,
        string  city,
        string  postalCode,
        string  countryCode,
        string? state = null)
    {
        _address = new AddressInformationBuilder()
            .WithStreet(street)
            .WithCity(city)
            .WithPostalCode(postalCode)
            .WithCountryCode(countryCode)
            .WithState(state);

        return this;
    }

    internal SubmodelElementCollection Build()
    {
        ValidateRequiredRoles();

        var contactInformation = new SubmodelElementCollection
                                 {
                                     IdShort    = DigitalNameplateIdentifiers.ContactInformationIdShort,
                                     SemanticId = ReferenceFactory.GlobalConceptDescription(ContactInformation)
                                 };

        var elements = new List<ISubmodelElement>();

        elements.Add(BuildManufacturerContact());

        elements.Add(CreateStringProperty(
            DigitalNameplateIdentifiers.ServiceHotlineIdShort,
            ServiceHotline,
            _serviceHotline!));

        if (!string.IsNullOrWhiteSpace(_email))
        {
            elements.Add(CreateStringProperty(
                DigitalNameplateIdentifiers.EmailIdShort,
                EmailAddress,
                _email!));
        }

        if (!string.IsNullOrWhiteSpace(_websiteUrl))
        {
            elements.Add(CreateStringProperty(
                DigitalNameplateIdentifiers.WebsiteUrlIdShort,
                WebsiteUrl,
                _websiteUrl!));
        }

        if (_address is not null)
        {
            elements.Add(_address.Build());
        }

        contactInformation.Value = elements;
        return contactInformation;
    }

    private SubmodelElementCollection BuildManufacturerContact()
    {
        var manufacturerContact = new SubmodelElementCollection
                                  {
                                      IdShort    = DigitalNameplateIdentifiers.ManufacturerContactIdShort,
                                      SemanticId = ReferenceFactory.GlobalConceptDescription(ManufacturerContact)
                                  };

        var values = new List<ISubmodelElement>
                     {
                         CreateStringProperty(
                             DigitalNameplateIdentifiers.ContactRoleIdShort,
                             ContactRole,
                             "Manufacturer"),
                         CreateStringProperty(
                             DigitalNameplateIdentifiers.ContactNameIdShort,
                             ContactName,
                             _manufacturerName!)
                     };

        if (!string.IsNullOrWhiteSpace(_manufacturerPhone))
        {
            values.Add(CreateStringProperty(
                DigitalNameplateIdentifiers.PhoneIdShort,
                Phone,
                _manufacturerPhone!));
        }

        manufacturerContact.Value = values;
        return manufacturerContact;
    }

    private void ValidateRequiredRoles()
    {
        if (string.IsNullOrWhiteSpace(_manufacturerName))
        {
            throw new InvalidOperationException(
                "Contact information is incomplete. Missing required contact role: ManufacturerContact.");
        }

        if (string.IsNullOrWhiteSpace(_serviceHotline))
        {
            throw new InvalidOperationException(
                "Contact information is incomplete. Missing required contact role: ServiceHotline.");
        }
    }

    private static void ValidatePhone(string phone, string paramName)
    {
        if (phone.Length < 3 || phone.Any(char.IsLetter))
        {
            throw new ArgumentException("Phone number format is invalid.", paramName);
        }
    }

    private static Property CreateStringProperty(string idShort, string semanticId, string value)
    {
        return new Property(
            idShort: idShort,
            category: null,
            semanticId: ReferenceFactory.GlobalConceptDescription(semanticId),
            valueType: DataTypeDefXsd.String,
            value: value);
    }

    private sealed class AddressInformationBuilder
    {
        private string? _street;
        private string? _city;
        private string? _postalCode;
        private string? _countryCode;
        private string? _state;

        public AddressInformationBuilder WithStreet(string street)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                throw new ArgumentException("Street must not be empty.", nameof(street));
            }

            _street = street;
            return this;
        }

        public AddressInformationBuilder WithCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City must not be empty.", nameof(city));
            }

            _city = city;
            return this;
        }

        public AddressInformationBuilder WithPostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new ArgumentException("Postal code must not be empty.", nameof(postalCode));
            }

            _postalCode = postalCode;
            return this;
        }

        public AddressInformationBuilder WithCountryCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                throw new ArgumentException("Country code must not be empty.", nameof(countryCode));
            }

            if (countryCode.Length != 2)
            {
                throw new ArgumentException("Country code must be a two-letter ISO code.", nameof(countryCode));
            }

            _countryCode = countryCode.ToUpperInvariant();
            return this;
        }

        public AddressInformationBuilder WithState(string? state)
        {
            if (!string.IsNullOrWhiteSpace(state))
            {
                _state = state;
            }

            return this;
        }

        public SubmodelElementCollection Build()
        {
            if (string.IsNullOrWhiteSpace(_street) ||
                string.IsNullOrWhiteSpace(_city) ||
                string.IsNullOrWhiteSpace(_postalCode) ||
                string.IsNullOrWhiteSpace(_countryCode))
            {
                throw new InvalidOperationException(
                    "Address information is incomplete. Street, City, PostalCode, and CountryCode are required.");
            }

            var address = new SubmodelElementCollection
                          {
                              IdShort    = DigitalNameplateIdentifiers.AddressInformationIdShort,
                              SemanticId = ReferenceFactory.GlobalConceptDescription(ContactAddressInformation)
                          };

            var values = new List<ISubmodelElement>
                         {
                             CreateStringProperty(
                                 DigitalNameplateIdentifiers.StreetIdShort,
                                 AddressStreet,
                                 _street!),
                             CreateStringProperty(
                                 DigitalNameplateIdentifiers.CityIdShort,
                                 AddressCity,
                                 _city!),
                             CreateStringProperty(
                                 DigitalNameplateIdentifiers.PostalCodeIdShort,
                                 AddressPostalCode,
                                 _postalCode!),
                             CreateStringProperty(
                                 DigitalNameplateIdentifiers.CountryCodeIdShort,
                                 AddressCountryCode,
                                 _countryCode!)
                         };

            if (!string.IsNullOrWhiteSpace(_state))
            {
                values.Add(CreateStringProperty(
                    DigitalNameplateIdentifiers.StateIdShort,
                    AddressState,
                    _state!));
            }

            address.Value = values;
            return address;
        }
    }
}
