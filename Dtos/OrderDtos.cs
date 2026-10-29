using System.ComponentModel.DataAnnotations;
using RiceDirectApi.Models;

namespace RiceDirectApi.Dtos;

public record PlaceOrderDto(
    [Required] int ListingId,
    [Range(1, 100000)] decimal QuantityKg,
    [Required] string DeliveryAddress,
    [Required, Phone] string ContactPhone
);

public record UpdateOrderStatusDto(
    [Required] OrderStatus NewStatus
);

public record OrderResponseDto(
    int OrderId,
    int ListingId,
    string VarietyName,
    decimal QuantityKg,
    decimal AgreedPricePerKg,
    decimal TotalAmount,
    string Status,
    string BuyerName,
    string BuyerPhone,
    string FarmerName,
    string FarmerPhone,
    string DeliveryAddress,
    DateTime CreatedAt
);