using SoftUniProject.Data.Models.Enums;
using SoftUniProject.ViewModels;

namespace SoftUniProject.Services;

public interface IOrderService
{
    // Create
    Task<Guid> CreateOrderAsync(CreateOrderViewModel model, string customerId);
    
    // Read
    Task<OrderDetailsViewModel?> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderListViewModel>> GetOrdersByCustomerIdAsync(string customerId);
    Task<IEnumerable<OrderListViewModel>> GetPendingOrdersAsync(int count = 10);
    Task<IEnumerable<OrderListViewModel>> GetOrdersByDeliveryManIdAsync(string deliveryManId);
    
    // Update
    Task<bool> AcceptOrderAsync(Guid orderId, string deliveryManId);
    Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<bool> CancelOrderAsync(Guid orderId, string userId);
    
    // Price calculation
    Task<decimal> CalculatePriceAsync(string pickupAddress, string deliveryAddress);
    
    // Validation
    Task<bool> CanUserAccessOrderAsync(Guid orderId, string userId);
    Task<bool> IsOrderAvailableForAcceptanceAsync(Guid orderId);
}