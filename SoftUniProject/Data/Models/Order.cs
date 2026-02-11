using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftUniProject.Data.Models.Enums;

namespace SoftUniProject.Data.Models;

public sealed class Order
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string PickupAddress { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string DeliveryAddress { get; set; } = null!;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [MaxLength(450)]
    public string CustomerId { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public ApplicationUser Customer { get; set; } = null!;

    [MaxLength(450)]
    public string? DeliveryManId { get; set; }

    [ForeignKey(nameof(DeliveryManId))]
    public ApplicationUser? DeliveryMan { get; set; }

    public Price? Price { get; set; }
}
