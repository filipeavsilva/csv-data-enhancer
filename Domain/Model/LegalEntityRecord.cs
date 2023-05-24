namespace Domain.Model;

public record LegalEntityRecord(LEI Identifier, string LegalName, Country Country, string BIC);