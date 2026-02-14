using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoftUniProject.Data.Models;
using SoftUniProject.Data.Models.Enums;
using SoftUniProject.Services;
using SoftUniProject.ViewModels;

namespace SoftUniProject.Controllers;

[Authorize]
public class DeliveriesController : Controller
{
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeliveriesController(IOrderService orderService, UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _userManager = userManager;
    }

    private async Task<bool> IsUserDeliveryMan()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.IsDeliveryMan ?? false;
    }

    public async Task<IActionResult> Available()
    {
        if (!await IsUserDeliveryMan()) return Forbid();

        var orders = await _orderService.GetPendingOrdersAsync();
        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(Guid id)
    {
        if (!await IsUserDeliveryMan()) return Forbid();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var success = await _orderService.AcceptOrderAsync(id, userId);
        if (!success)
        {
            TempData["Error"] = "Could not accept the order. It might have been already accepted or cancelled.";
            return RedirectToAction(nameof(Available));
        }

        TempData["Success"] = "Order accepted successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        if (!await IsUserDeliveryMan()) return Forbid();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var orders = await _orderService.GetOrdersByDeliveryManIdAsync(userId);
        return View(orders);
    }

    public async Task<IActionResult> Earnings()
    {
        if (!await IsUserDeliveryMan()) return Forbid();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var completedDeliveries = await _orderService.GetCompletedDeliveriesAsync(userId);
        
        var viewModel = new EarningsViewModel
        {
            CompletedDeliveries = completedDeliveries,
            TotalEarnings = completedDeliveries.Sum(o => o.PriceAmount)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        if (!await IsUserDeliveryMan()) return Forbid();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        if (!await _orderService.CanUserAccessOrderAsync(id, userId))
        {
            return Forbid();
        }

        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus status)
    {
        if (!await IsUserDeliveryMan()) return Forbid();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        // Check if the order belongs to this delivery man
        if (!await _orderService.CanUserAccessOrderAsync(id, userId))
        {
            return Forbid();
        }

        var success = await _orderService.UpdateOrderStatusAsync(id, status);
        if (!success)
        {
            TempData["Error"] = "Could not update order status.";
        }
        else
        {
            TempData["Success"] = $"Order status updated to {status}.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
