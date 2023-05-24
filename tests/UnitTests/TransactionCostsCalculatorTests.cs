using Domain;
using Domain.Model;
using FluentAssertions;
using NodaTime;
using NodaTime.TimeZones;

namespace UnitTests;

public class TransactionCostsCalculatorTests
{
    private readonly UTI _testUti = new("1030291281MARKITWIRE0000000000000112874138");
    private readonly ISIN _testIsin = new("EZD7JRS42975");
    private readonly LEI _testLei = new("BFXS5XCH7N0Y05NIXW11");
    private const string TestBIC = "SGEGDEF1XXX";

    [Theory]
    [InlineData(Country.GB, 1, 1, 0)]
    [InlineData(Country.GB, 0, 1, 0)]
    [InlineData(Country.GB, 2.345, 0.786, -0.50183)]
    [InlineData(Country.GB, 0.753, 0.00321, -0.75058287)]
    [InlineData(Country.NL, 1, 1, 0)]
    [InlineData(Country.NL, 0, 1, 0)]
    [InlineData(Country.NL, 2.345, 0.786, 0.6384605597)]
    [InlineData(Country.NL, 0.753, 0.00321, 233.826439252)]
    public void WithExpectedCountries_CalculateTransactionCosts_ReturnsExpectedValue(Country country, decimal notionalValue, decimal rate, decimal expectedTransactionCosts)
    {
        var transaction = new Transaction(_testUti, _testIsin, notionalValue, Currency.EUR, Instant.MinValue, rate, _testLei);
        var legalEntityRecord = new LegalEntityRecord(_testLei, string.Empty, country, TestBIC);

        TransactionCostsCalculator.CalculateTransactionCosts(transaction, legalEntityRecord)
            .Should().BeApproximately(expectedTransactionCosts, precision: 0.000000001m);
    }

    [Theory]
    [InlineData(Country.GB, 0)]
    [InlineData(Country.GB, -1)]
    [InlineData(Country.NL, 0)]
    [InlineData(Country.NL, -1)]
    public void WithZeroOrNegativeRate_CalculateTransactionCosts_ThrowsException(Country country, decimal rate)
    {
        var transaction = new Transaction(_testUti, _testIsin, 4.56m, Currency.EUR, Instant.MinValue, rate, _testLei);
        var legalEntityRecord = new LegalEntityRecord(_testLei, string.Empty, country, TestBIC);

        Action act = () => TransactionCostsCalculator.CalculateTransactionCosts(transaction, legalEntityRecord);

        act.Should().Throw<ArgumentOutOfRangeException>("because a zero rate is invalid and can result in incorrect calculations").WithParameterName(nameof(transaction.Rate));
    }

    [Fact]
    public void WithUnsupportedCountry_CalculateTransactionCosts_ThrowsException()
    {
        var transaction = new Transaction(_testUti, _testIsin, 4.56m, Currency.EUR, Instant.MinValue, 0.78m, _testLei);
        var legalEntityRecord = new LegalEntityRecord(_testLei, string.Empty, Country.DE, TestBIC);

        Action act = () => TransactionCostsCalculator.CalculateTransactionCosts(transaction, legalEntityRecord);

        act.Should().Throw<ArgumentOutOfRangeException>("because DE is not a supported country").WithParameterName(nameof(legalEntityRecord.Country));
    }
}