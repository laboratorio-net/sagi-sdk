namespace Samples.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public float Amount { get; set; }

        private static string RandomStatus()
        {
            var statusList = new string[] { "Pending", "Processing", "Done" };
            return statusList[Random.Shared.Next(0, statusList.Length - 1)];
        }

        private static float RandomAmount() =>
            Random.Shared.Next(1, 1_000_0000) * (Random.Shared.Next(1, 100) / 100);

        public static Order Create() => new()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = RandomStatus(),
            Amount = RandomAmount()
        };
    }
}
