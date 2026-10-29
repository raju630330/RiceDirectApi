namespace RiceDirectApi.Models;

public enum OrderStatus
{
    Pending,
    AcceptedByFarmer,
    Rejected,
    Dispatched,
    Delivered,
    Cancelled
}

public class Order
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public RiceListing? Listing { get; set; }

    public int BuyerId { get; set; }
    public User? Buyer { get; set; }

    public decimal QuantityKg { get; set; }
    public decimal AgreedPricePerKg { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}