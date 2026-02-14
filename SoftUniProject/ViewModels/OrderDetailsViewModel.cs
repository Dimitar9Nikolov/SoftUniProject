using SoftUniProject.Data.Models.Enums;

namespace SoftUniProject.ViewModels;

public class OrderDetailsViewModel
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public string PickupAddress { get; set; } = null!;

    public string DeliveryAddress { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public OrderStatus Status { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string? DeliveryManName { get; set; }

    public string? DeliveryManPhone { get; set; }

    public decimal PriceAmount { get; set; }

    public double Distance { get; set; }

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
