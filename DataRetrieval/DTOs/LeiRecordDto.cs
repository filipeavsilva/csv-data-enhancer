namespace DataRetrieval.DTOs;

public record LeiRecordDto(string Id, LeiRecordAttributesDto Attributes);
public record LeiRecordAttributesDto(string Lei, LegalEntityDto Entity, List<string> Bic);
public record LegalEntityDto(EntityNameDto LegalName, EntityAddressDto LegalAddress);
public record EntityNameDto(string Name);
public record EntityAddressDto(string Country);