using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiceDirectApi.Data;
using RiceDirectApi.Dtos;
using RiceDirectApi.Models;

namespace RiceDirectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ListingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? variety,
        [FromQuery] string? district,
        [FromQuery] decimal? maxPrice)
    {
        var query = _context.RiceListings
            .Include(l => l.Farmer)
            .Where(l => l.IsActive && l.AvailableQuantityKg > 0);

        if (!string.IsNullOrWhiteSpace(variety))
            query = query.Where(l => EF.Functions.Like(l.VarietyName.ToLower(), $"%{variety.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(district))
            query = query.Where(l => EF.Functions.Like(l.Farmer!.District.ToLower(), $"%{district.ToLower()}%"));

        if (maxPrice.HasValue)
            query = query.Where(l => l.PricePerKg <= maxPrice.Value);

        var list = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new ListingResponseDto(
                l.Id,
                l.FarmerId,
                l.Farmer!.FullName,
                l.Farmer.PhoneNumber,
                l.Farmer.District,
                l.Farmer.State,
                l.VarietyName,
                l.ProcessingType.ToString(),
                l.AvailableQuantityKg,
                l.MinOrderQuantityKg,
                l.PricePerKg,
                l.SampleAvailable,
                l.ImageUrl,
                l.Description,
                l.CreatedAt
            ))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var l = await _context.RiceListings.Include(x => x.Farmer).FirstOrDefaultAsync(x => x.Id == id);
        if (l == null) return NotFound();

        return Ok(new ListingResponseDto(
            l.Id, l.FarmerId, l.Farmer!.FullName, l.Farmer.PhoneNumber,
            l.Farmer.District, l.Farmer.State, l.VarietyName,
            l.ProcessingType.ToString(), l.AvailableQuantityKg,
            l.MinOrderQuantityKg, l.PricePerKg, l.SampleAvailable,
            l.ImageUrl, l.Description, l.CreatedAt
        ));
    }

    [Authorize(Roles = "Farmer")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var listing = new RiceListing
        {
            FarmerId = userId,
            VarietyName = dto.VarietyName,
            ProcessingType = dto.ProcessingType,
            TotalQuantityKg = dto.TotalQuantityKg,
            AvailableQuantityKg = dto.TotalQuantityKg,
            MinOrderQuantityKg = dto.MinOrderQuantityKg,
            PricePerKg = dto.PricePerKg,
            SampleAvailable = dto.SampleAvailable,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description
        };

        _context.RiceListings.Add(listing);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }
}