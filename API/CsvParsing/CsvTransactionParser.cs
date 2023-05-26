using System.Globalization;
using System.Runtime.CompilerServices;
using API.DTOs;
using CsvHelper;
using Domain.Model;

namespace API.CsvParsing;

public class CsvTransactionParser
{
    public async IAsyncEnumerable<Transaction> ParseCsvFile(Stream csvStream, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(csvStream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        var csvTransactions = csvReader.GetRecordsAsync<CsvTransactionDto>(cancellationToken);
        if (csvTransactions is null)
        {
            throw new ArgumentException("Could not parse csv stream correctly");
        }

        var transactionEnumerator = csvTransactions.GetAsyncEnumerator(cancellationToken);
        for (var hasTransactions = true; hasTransactions;)
        {
            try
            {
                hasTransactions = await transactionEnumerator.MoveNextAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not parse csv stream correctly", ex);
            }

            if (hasTransactions)
            {
                yield return transactionEnumerator.Current.Map();
            }
        }
    }
}