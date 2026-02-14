using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoftUniProject.Data.Models;
using SoftUniProject.Services;
using SoftUniProject.ViewModels;

namespace SoftUniProject.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(IOrderService orderService, UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var orders = await _orderService.GetOrdersByCustomerIdAsync(userId);
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        if (!await _orderService.CanUserAccessOrderAsync(id, userId))
        {
            return Forbid();
        }

        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateOrderViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var orderId = await _orderService.CreateOrderAsync(model, userId);
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var success = await _orderService.CancelOrderAsync(id, userId);
        if (!success)
        {
            return BadRequest("Could not cancel the order.");
        }

        return RedirectToAction(nameof(Index));
    }
}