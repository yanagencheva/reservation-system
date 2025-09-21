using Reservation.Persistence.Entities;
using SwiftProcessor.Worker.Models;
using System.Globalization;

namespace SwiftProcessor.Worker.Utils;

public static class SwiftMessageMapper
{
    public static SwiftMetadata ToEntity(SwiftMessage msg)
    {
        var entity = new SwiftMetadata
        {
            BasicHeader = msg.RawBasicHeader,
            ApplicationHeader = msg.RawApplicationHeader,
            UserHeader = msg.RawUserHeader,
            TextBlock = msg.RawTextBlock,
            Trailer = msg.RawTrailer,
            IsMt103Plus = msg.IsMt103Plus()
        };

        // :20: Transaction reference
        entity.TransactionReference = msg.GetField("20").FirstOrDefault();

        // :32A: Value date + currency + amount
        var field32A = msg.GetField("32A").FirstOrDefault();
        if (!string.IsNullOrEmpty(field32A))
        {
            // Format is YYMMDDCCCAMOUNT, e.g. "250921EUR12345,67"
            if (field32A.Length >= 6)
            {
                var datePart = field32A.Substring(0, 6);
                if (DateTimeOffset.TryParseExact(
                    datePart,
                    "yyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var valueDate))
                {
                    entity.ValueDate = valueDate;
                }
            }

            if (field32A.Length >= 9)
            {
                entity.Currency = field32A.Substring(6, 3);
                entity.Amount = field32A.Substring(9);
            }
        }

        // :59: Beneficiary
        entity.Beneficiary = msg.GetField("59").FirstOrDefault();

        // :50A/K/F: Ordering Customer
        var field50 = msg.GetField("50A").FirstOrDefault()
                     ?? msg.GetField("50K").FirstOrDefault()
                     ?? msg.GetField("50F").FirstOrDefault();
        entity.OrderingCustomer = field50;

        // Trailer fields
        if (msg.TrailerFields.TryGetValue("MAC", out var mac))
            entity.Mac = mac;
        if (msg.TrailerFields.TryGetValue("CHK", out var chk))
            entity.Checksum = chk;

        return entity;
    }
}
