namespace Domain.Model;

public record LegalEntityRecord(LEI Identifier, string LegalName, Country Country, List<string> BICs);