namespace RiceDirectApi.Models;

public enum RiceType
{
    Raw,
    Steamed,
    Parboiled,
    Paddy
}

public class RiceListing
{
    public int Id { get; set; }
    public int FarmerId { get; set; }
    public User? Farmer { get; set; }

    public string VarietyName { get; set; } = string.Empty;
    public RiceType ProcessingType { get; set; } = RiceType.Raw;
    public decimal TotalQuantityKg { get; set; }
    public decimal AvailableQuantityKg { get; set; }
    public decimal MinOrderQuantityKg { get; set; } = 25.0m;
    public decimal PricePerKg { get; set; }
    public bool SampleAvailable { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}