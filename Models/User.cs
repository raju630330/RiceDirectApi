namespace RiceDirectApi.Models;

public enum UserRole
{
    Farmer,
    Buyer,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string District { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string VillageOrCity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RiceListing> Listings { get; set; } = new List<RiceListing>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}