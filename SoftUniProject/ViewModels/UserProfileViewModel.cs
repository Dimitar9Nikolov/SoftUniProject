using System.ComponentModel.DataAnnotations;

namespace SoftUniProject.ViewModels;

public class UserProfileViewModel
{
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;

    public bool IsDeliveryMan { get; set; }

    // Statistics
    public int TotalOrdersPlaced { get; set; }

    public int TotalDeliveriesCompleted { get; set; }

    public int ActiveOrders { get; set; }

    public int ActiveDeliveries { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
