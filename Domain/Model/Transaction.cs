using NodaTime;
using StronglyTypedIds;

// Set the defaults for the project
[assembly:StronglyTypedIdDefaults(
    converters: StronglyTypedIdConverter.SystemTextJson)]


namespace Domain.Model;

// ReSharper disable InconsistentNaming (Reason: This file includes initialisms such as LEI and UTI that are more recognizable capitalized than if following PascalCase conventions)

public record Transaction(UTI Id, ISIN ISIN, decimal NotionalValue, Currency NotionalCurrency, Instant Timestamp, decimal Rate, LEI EntityId);

/// <summary> A Universal Transaction Identifier (for more information, see https://www.swift.com/your-needs/capital-markets/unique-transaction-identifier-securities-all-you-need-know)</summary>
[StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
public partial struct UTI { }

///<summary> A Legal Entity Identifier (for more information, see https://www.gleif.org/en/about-lei/introducing-the-legal-entity-identifier-lei)</summary>
[StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
public partial struct LEI { }

/// <summary> An International Securities Identification Number (for more information, see https://www.investopedia.com/terms/i/isin.asp)</summary>
[StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
public partial struct ISIN { }