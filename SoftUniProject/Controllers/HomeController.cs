using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniProject.Data;
using SoftUniProject.Data.Models;
using SoftUniProject.Data.Models.Enums;
using SoftUniProject.Services;
using SoftUniProject.ViewModels;

namespace SoftUniProject.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOrderService _orderService;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOrderService orderService)
    {
        _context = context;
        _userManager = userManager;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveDeliveryMenCount = await _orderService.GetActiveDeliveryMenCountAsync();

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (user.IsDeliveryMan)
                {
                    var availableOrders = await _context.Orders
                        .Where(o => o.Status == OrderStatus.Pending)
                        .Include(o => o.Customer)
                        .OrderByDescending(o => o.CreatedOn)
                        .Take(10)
                        .ToListAsync();
                    
                    ViewBag.AvailableOrders = availableOrders;
                }
                else
                {
                    var myOrders = await _context.Orders
                        .Where(o => o.CustomerId == user.Id)
                        .OrderByDescending(o => o.CreatedOn)
                        .Take(5)
                        .ToListAsync();
                    
                    ViewBag.MyOrders = myOrders;
                }
            }
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}