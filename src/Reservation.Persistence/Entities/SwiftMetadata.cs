using System.ComponentModel.DataAnnotations;

namespace Reservation.Persistence.Entities;

public class SwiftMetadata : Entity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Raw Basic Header block ({1:...}).</summary>
    [MaxLength(500)]
    public string? BasicHeader { get; set; }

    /// <summary>Raw Application Header block ({2:...}).</summary>
    [MaxLength(1000)]
    public string? ApplicationHeader { get; set; }

    /// <summary>Raw User Header block ({3:...}).</summary>
    [MaxLength(1000)]
    public string? UserHeader { get; set; }

    /// <summary>Raw Text Block ({4:...}).</summary>
    public string? TextBlock { get; set; }

    /// <summary>Raw Trailer block ({5:...}).</summary>
    [MaxLength(1000)]
    public string? Trailer { get; set; }

    /// <summary>Флаг дали съобщението е MT103+ (Block 3 съдържа {119:STP}).</summary>
    public bool IsMt103Plus { get; set; }

    /// <summary>Transaction Reference (:20: поле от Block 4).</summary>
    [MaxLength(50)]
    public string? TransactionReference { get; set; }

    /// <summary>Amount (:32A: поле от Block 4).</summary>
    [MaxLength(50)]
    public string? Amount { get; set; }

    /// <summary>Currency (:32A: поле от Block 4).</summary>
    [MaxLength(3)]
    public string? Currency { get; set; }

    /// <summary>Value Date (:32A: поле от Block 4).</summary>
    public DateTimeOffset? ValueDate { get; set; }

    /// <summary>Beneficiary (:59: поле от Block 4).</summary>
    public string? Beneficiary { get; set; }

    /// <summary>Ordering Customer (:50A/K/F: поле от Block 4).</summary>
    public string? OrderingCustomer { get; set; }

    /// <summary>MAC от Trailer.</summary>
    [MaxLength(50)]
    public string? Mac { get; set; }

    /// <summary>CHK от Trailer.</summary>
    [MaxLength(50)]
    public string? Checksum { get; set; }
}
