using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RiceDirectApi.Data;
using RiceDirectApi.Dtos;
using RiceDirectApi.Models;

namespace RiceDirectApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
            return null;

        var user = new User
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            VillageOrCity = dto.VillageOrCity,
            District = dto.District,
            State = dto.State
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, user.Id, user.FullName, user.Role.ToString());
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, user.Id, user.FullName, user.Role.ToString());
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("PhoneNumber", user.PhoneNumber)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}