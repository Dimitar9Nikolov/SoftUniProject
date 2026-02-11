using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SoftUniProject.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public override string Email { get; set; } = null!;

    [Required]
    [Phone]
    public override string PhoneNumber { get; set; } = null!;

    public bool IsDeliveryMan { get; set; }

    // Navigation properties
    public virtual ICollection<Order> PlacedOrders { get; set; } = new List<Order>();
    public virtual ICollection<Order> AcceptedDeliveries { get; set; } = new List<Order>();
}
