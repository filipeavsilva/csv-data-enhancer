using DataRetrieval.DTOs;
using Domain.Model;

namespace DataRetrieval;

public static class DtoMappingExtensions
{
    public static LegalEntityRecord Map(this LeiRecordDto dto)
    {
        var countryParsed = Enum.TryParse(dto.Attributes.Entity.LegalAddress.Country, out Country country);
        if (!countryParsed)
        {
            country = Country.UNSUPPORTED;
        }

        return new LegalEntityRecord(new LEI(dto.Attributes.Lei), dto.Attributes.Entity.LegalName.Name, country, dto.Attributes.Bic ?? new List<string>());
    }
}