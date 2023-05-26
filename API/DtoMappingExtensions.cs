using API.DTOs;
using Domain.Model;
using NodaTime;

namespace API;

public static class DtoMappingExtensions
{
    public static Transaction Map(this CsvTransactionDto dto)
    {
        var dateTimeOffset = DateTimeOffset.Parse(dto.TransactionDateTime);
        return new Transaction(new UTI(dto.TransactionIdentifier), new ISIN(dto.ISIN), dto.NotionalValue, dto.NotionalCurrency,
            dto.TransactionType, Instant.FromDateTimeOffset(dateTimeOffset), dto.Rate, new LEI(dto.LEI));
    }
}