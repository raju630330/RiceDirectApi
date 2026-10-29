using System.ComponentModel.DataAnnotations;
using RiceDirectApi.Models;

namespace RiceDirectApi.Dtos;

public record RegisterRequestDto(
    [Required] string FullName,
    [Required, Phone] string PhoneNumber,
    [Required, MinLength(4)] string Password,
    [Required] UserRole Role,
    string VillageOrCity,
    string District,
    string State
);

public record LoginRequestDto(
    [Required, Phone] string PhoneNumber,
    [Required] string Password
);

public record AuthResponseDto(
    string Token,
    int UserId,
    string FullName,
    string Role
);