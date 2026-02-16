using Microsoft.EntityFrameworkCore;
using SoftUniProject.Data;
using SoftUniProject.Data.Models;
using SoftUniProject.Data.Models.Enums;
using SoftUniProject.ViewModels;

namespace SoftUniProject.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private const decimal PricePerKm = 1m;
    private const decimal BaseFee = 2.0m;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderViewModel model, string customerId)
    {
        var order = new Order
        {
            Description = model.Description,
            PickupAddress = model.PickupAddress,
            DeliveryAddress = model.DeliveryAddress,
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            RequiresPayment = model.RequiresPayment,
            EstimatedPickupPrice = model.RequiresPayment ? model.EstimatedPickupPrice : null,
            PaymentMethod = model.PaymentMethod,
            CreatedOn = DateTime.UtcNow
        };

        // Calculate price
        var priceAmount = await CalculatePriceAsync(model.PickupAddress, model.DeliveryAddress);
        var distance = CalculateDistance(model.PickupAddress, model.DeliveryAddress);

        var price = new Price
        {
            Amount = priceAmount,
            Distance = distance,
            OrderId = order.Id
        };

        _context.Orders.Add(order);
        _context.Prices.Add(price);
        await _context.SaveChangesAsync();

        return order.Id;
    }

    public async Task<OrderDetailsViewModel?> GetOrderByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.Customer)
            .Include(o => o.DeliveryMan)
            .Include(o => o.Price)
            .Select(o => new OrderDetailsViewModel
            {
                Id = o.Id,
                Description = o.Description,
                PickupAddress = o.PickupAddress,
                DeliveryAddress = o.DeliveryAddress,
                Status = o.Status,
                CreatedOn = o.CreatedOn,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                CustomerPhone = o.Customer.PhoneNumber,
                DeliveryManName = o.DeliveryMan != null
                    ? $"{o.DeliveryMan.FirstName} {o.DeliveryMan.LastName}"
                    : null,
                DeliveryManPhone = o.DeliveryMan != null ? o.DeliveryMan.PhoneNumber : null,
                PriceAmount = o.Price != null ? o.Price.Amount : 0,
                Distance = o.Price != null ? o.Price.Distance : 0,
                RequiresPayment = o.RequiresPayment,
                EstimatedPickupPrice = o.EstimatedPickupPrice,
                PaymentMethod = o.PaymentMethod
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<OrderListViewModel>> GetOrdersByCustomerIdAsync(string customerId)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Price)
            .OrderByDescending(o => o.CreatedOn)
            .Select(o => new OrderListViewModel
            {
                Id = o.Id,
                Description = o.Description,
                Status = o.Status,
                CreatedOn = o.CreatedOn,
                PriceAmount = o.Price != null ? o.Price.Amount : 0,
                RequiresPayment = o.RequiresPayment
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderListViewModel>> GetPendingOrdersAsync(int count = 10)
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .Include(o => o.Customer)
            .Include(o => o.Price)
            .OrderByDescending(o => o.CreatedOn)
            .Take(count)
            .Select(o => new OrderListViewModel
            {
                Id = o.Id,
                Description = o.Description,
                PickupAddress = o.PickupAddress,
                DeliveryAddress = o.DeliveryAddress,
                Status = o.Status,
                CreatedOn = o.CreatedOn,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                PriceAmount = o.Price != null ? o.Price.Amount : 0,
                RequiresPayment = o.RequiresPayment,
                EstimatedPickupPrice = o.EstimatedPickupPrice
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderListViewModel>> GetOrdersByDeliveryManIdAsync(string deliveryManId)
    {
        return await _context.Orders
            .Where(o => o.DeliveryManId == deliveryManId)
            .Include(o => o.Price)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedOn)
            .Select(o => new OrderListViewModel
            {
                Id = o.Id,
                Description = o.Description,
                PickupAddress = o.PickupAddress,
                DeliveryAddress = o.DeliveryAddress,
                Status = o.Status,
                CreatedOn = o.CreatedOn,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                PriceAmount = o.Price != null ? o.Price.Amount : 0,
                RequiresPayment = o.RequiresPayment,
                EstimatedPickupPrice = o.EstimatedPickupPrice
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderListViewModel>> GetCompletedDeliveriesAsync(string deliveryManId)
    {
        return await _context.Orders
            .Where(o => o.DeliveryManId == deliveryManId && o.Status == OrderStatus.Delivered)
            .Include(o => o.Price)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedOn)
            .Select(o => new OrderListViewModel
            {
                Id = o.Id,
                Description = o.Description,
                PickupAddress = o.PickupAddress,
                DeliveryAddress = o.DeliveryAddress,
                Status = o.Status,
                CreatedOn = o.CreatedOn,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                PriceAmount = o.Price != null ? o.Price.Amount : 0,
                RequiresPayment = o.RequiresPayment,
                EstimatedPickupPrice = o.EstimatedPickupPrice
            })
            .ToListAsync();
    }

    public async Task<int> GetActiveDeliveryMenCountAsync()
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        return await _context.Orders
            .Where(o => o.AcceptedOn >= oneHourAgo && o.DeliveryManId != null)
            .Select(o => o.DeliveryManId)
            .Distinct()
            .CountAsync();
    }

    public async Task<bool> AcceptOrderAsync(Guid orderId, string deliveryManId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null || order.Status != OrderStatus.Pending)
            return false;

        order.DeliveryManId = deliveryManId;
        order.Status = OrderStatus.Accepted;
        order.AcceptedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
            return false;

        order.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, string userId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null || order.CustomerId != userId)
            return false;

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            return false;

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<decimal> CalculatePriceAsync(string pickupAddress, string deliveryAddress)
    {
        // For now, use simplified calculation
        // In production, integrate with Google Maps API or similar
        var distance = CalculateDistance(pickupAddress, deliveryAddress);
        var price = BaseFee + (decimal)distance * PricePerKm;
        return Task.FromResult(price);
    }

    public async Task<bool> CanUserAccessOrderAsync(Guid orderId, string userId)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == orderId &&
                          (o.CustomerId == userId || o.DeliveryManId == userId));
    }

    public async Task<bool> IsOrderAvailableForAcceptanceAsync(Guid orderId)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == orderId && o.Status == OrderStatus.Pending);
    }

    // Private helper method for distance calculation
    private double CalculateDistance(string pickupAddress, string deliveryAddress)
    {
        // Simplified: return random distance between 1-20 km for testing
        // In production, use geocoding API (Google Maps Distance Matrix, etc.)
        var random = new Random(pickupAddress.GetHashCode() + deliveryAddress.GetHashCode());
        return Math.Round(random.NextDouble() * 19 + 1, 2);
    }
}
