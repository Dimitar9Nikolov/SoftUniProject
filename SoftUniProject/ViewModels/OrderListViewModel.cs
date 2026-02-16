using SoftUniProject.Data.Models.Enums;

namespace SoftUniProject.ViewModels;

public class OrderListViewModel
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public string? PickupAddress { get; set; }

    public string? DeliveryAddress { get; set; }

    public DateTime CreatedOn { get; set; }

    public OrderStatus Status { get; set; }

    public string? CustomerName { get; set; }

    public decimal PriceAmount { get; set; }

    public bool RequiresPayment { get; set; }

    public string FormattedCreatedOn => CreatedOn.ToString("MMM dd, HH:mm");

    public string FormattedPrice => $"€{PriceAmount:F2}";

    public string StatusDisplayClass => Status switch
    {
        OrderStatus.Pending => "warning",
        OrderStatus.Accepted => "info",
        OrderStatus.InTransit => "primary",
        OrderStatus.Delivered => "success",
        OrderStatus.Cancelled => "danger",
        _ => "secondary"
    };
}
