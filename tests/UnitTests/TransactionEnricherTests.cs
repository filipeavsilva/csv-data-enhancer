using Domain;
using Domain.Model;
using FluentAssertions;
using NodaTime;

namespace UnitTests;

public class TransactionEnricherTests
{
    [Fact]
    public void Enrich_AddsLegalNameAndBICFromLegalEntity()
    {
        var lei = new LEI("BFXS5XCH7N0Y05NIXW11");
        var transaction = new Transaction(new UTI("1030291281MARKITWIRE0000000000000112874138"), new ISIN("EZD7JRS42975"), 3.4m, Currency.EUR, Instant.MinValue, 0.098m, lei);
        var legalEntityRecord = new LegalEntityRecord(lei, string.Empty, Country.GB, "SGEGDEF1XXX");

        var sut = new TransactionEnricher();

        var enrichedTransaction = sut.Enrich(transaction, legalEntityRecord);

        enrichedTransaction.EntityName.Should().Be(legalEntityRecord.LegalName);
        enrichedTransaction.EntityBIC.Should().Be(legalEntityRecord.BIC);
    }
}