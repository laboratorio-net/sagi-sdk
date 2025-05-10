public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public float Amount { get; set; }
    public required string Type { get; set; }
    public DateTime CreatedAt { get; set; }

    private static string RandomType()
    {
        string[] type = ["Money", "Credit", "Debit"];
        return type[Random.Shared.Next(0, type.Length - 1)];
    }

    public static Payment Create(Guid orderId, float amount) => new()
    {
        Id = Guid.NewGuid(),
        OrderId = orderId,
        Amount = amount,
        Type = RandomType(),
        CreatedAt = DateTime.UtcNow,
    };
}