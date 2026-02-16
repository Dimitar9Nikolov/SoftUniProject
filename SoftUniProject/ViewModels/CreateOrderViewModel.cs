using SoftUniProject.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftUniProject.ViewModels;

public class CreateOrderViewModel
{
    [Required(ErrorMessage = "Please describe what you need delivered")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Description must be between 10 and 200 characters")]
    [Display(Name = "Order Description")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Pickup address is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Pickup address must be between 5 and 500 characters")]
    [Display(Name = "Pickup Address")]
    public string PickupAddress { get; set; } = null!;

    [Required(ErrorMessage = "Delivery address is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Delivery address must be between 5 and 500 characters")]
    [Display(Name = "Delivery Address")]
    public string DeliveryAddress { get; set; } = null!;

    [Display(Name = "Delivery man needs to pay for items at pickup?")]
    public bool RequiresPayment { get; set; }

    [Display(Name = "Estimated price of items")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
    public decimal? EstimatedPickupPrice { get; set; }

    [Required]
    [Display(Name = "Payment Method")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
}
