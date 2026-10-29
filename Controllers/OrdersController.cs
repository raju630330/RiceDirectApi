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
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        var buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var listing = await _context.RiceListings.FindAsync(dto.ListingId);
        if (listing == null || !listing.IsActive)
            return BadRequest(new { message = "Listing is not available." });

        if (dto.QuantityKg < listing.MinOrderQuantityKg)
            return BadRequest(new { message = $"Minimum order is {listing.MinOrderQuantityKg} kg." });

        if (dto.QuantityKg > listing.AvailableQuantityKg)
            return BadRequest(new { message = $"Only {listing.AvailableQuantityKg} kg available." });

        var order = new Order
        {
            BuyerId = buyerId,
            ListingId = dto.ListingId,
            QuantityKg = dto.QuantityKg,
            AgreedPricePerKg = listing.PricePerKg,
            TotalAmount = dto.QuantityKg * listing.PricePerKg,
            DeliveryAddress = dto.DeliveryAddress,
            ContactPhone = dto.ContactPhone,
            Status = OrderStatus.Pending
        };

        listing.AvailableQuantityKg -= dto.QuantityKg;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order request placed successfully.", orderId = order.Id });
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        var query = _context.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Listing)
                .ThenInclude(l => l!.Farmer)
            .AsQueryable();

        if (role == "Farmer")
            query = query.Where(o => o.Listing!.FarmerId == userId);
        else
            query = query.Where(o => o.BuyerId == userId);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderResponseDto(
                o.Id,
                o.ListingId,
                o.Listing!.VarietyName,
                o.QuantityKg,
                o.AgreedPricePerKg,
                o.TotalAmount,
                o.Status.ToString(),
                o.Buyer!.FullName,
                o.Buyer.PhoneNumber,
                o.Listing.Farmer!.FullName,
                o.Listing.Farmer.PhoneNumber,
                o.DeliveryAddress,
                o.CreatedAt
            ))
            .ToListAsync();

        return Ok(orders);
    }

    [Authorize(Roles = "Farmer")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var order = await _context.Orders
            .Include(o => o.Listing)
            .FirstOrDefaultAsync(o => o.Id == id && o.Listing!.FarmerId == farmerId);

        if (order == null) return NotFound(new { message = "Order not found." });

        if (dto.NewStatus == OrderStatus.Rejected || dto.NewStatus == OrderStatus.Cancelled)
        {
            if (order.Status != OrderStatus.Rejected && order.Status != OrderStatus.Cancelled)
            {
                order.Listing!.AvailableQuantityKg += order.QuantityKg;
            }
        }

        order.Status = dto.NewStatus;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order status updated.", status = order.Status.ToString() });
    }
}