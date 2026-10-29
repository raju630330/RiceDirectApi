using System.ComponentModel.DataAnnotations;
using RiceDirectApi.Models;

namespace RiceDirectApi.Dtos;

public record CreateListingDto(
    [Required] string VarietyName,
    RiceType ProcessingType,
    [Range(1, 1000000)] decimal TotalQuantityKg,
    [Range(1, 10000)] decimal MinOrderQuantityKg,
    [Range(1, 10000)] decimal PricePerKg,
    bool SampleAvailable,
    string ImageUrl,
    string Description
);

public record ListingResponseDto(
    int Id,
    int FarmerId,
    string FarmerName,
    string FarmerPhone,
    string District,
    string State,
    string VarietyName,
    string ProcessingType,
    decimal AvailableQuantityKg,
    decimal MinOrderQuantityKg,
    decimal PricePerKg,
    bool SampleAvailable,
    string ImageUrl,
    string Description,
    DateTime CreatedAt
);