using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SoftUniProject.Data.Models;
using SoftUniProject.Data.Models.Enums;

namespace SoftUniProject.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Seed Users
        if (!dbContext.Users.Any(u => u.UserName!.Contains("test.com")))
        {
            var users = new List<(ApplicationUser User, string Password)>
            {
                (new ApplicationUser { FirstName = "John", LastName = "Customer", Email = "customer1@test.com", UserName = "customer1@test.com", PhoneNumber = "111222333", EmailConfirmed = true, IsDeliveryMan = false }, "Test123!"),
                (new ApplicationUser { FirstName = "Jane", LastName = "Customer", Email = "customer2@test.com", UserName = "customer2@test.com", PhoneNumber = "444555666", EmailConfirmed = true, IsDeliveryMan = false }, "Test123!"),
                (new ApplicationUser { FirstName = "Bob", LastName = "Delivery", Email = "delivery1@test.com", UserName = "delivery1@test.com", PhoneNumber = "777888999", EmailConfirmed = true, IsDeliveryMan = true }, "Test123!"),
                (new ApplicationUser { FirstName = "Alice", LastName = "Delivery", Email = "delivery2@test.com", UserName = "delivery2@test.com", PhoneNumber = "000111222", EmailConfirmed = true, IsDeliveryMan = true }, "Test123!"),
                (new ApplicationUser { FirstName = "Charlie", LastName = "User", Email = "customer3@test.com", UserName = "customer3@test.com", PhoneNumber = "333444555", EmailConfirmed = true, IsDeliveryMan = false }, "Test123!")
            };

            foreach (var (user, password) in users)
            {
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    // Log or handle error if needed, but for seeding we hope it works
                    continue;
                }
            }
        }

        // 2. Seed Orders 
        if (!dbContext.Orders.Any(o => o.Customer.UserName!.Contains("test.com")))
        {
            var customer1 = await userManager.FindByEmailAsync("customer1@test.com");
            var customer2 = await userManager.FindByEmailAsync("customer2@test.com");
            var delivery1 = await userManager.FindByEmailAsync("delivery1@test.com");

            if (customer1 != null && customer2 != null && delivery1 != null)
            {
                var orders = new List<Order>
                {
                    new Order
                    {
                        Description = "Groceries from local store",
                        PickupAddress = "123 Market St",
                        DeliveryAddress = "456 Home Ave",
                        Status = OrderStatus.Pending,
                        CustomerId = customer1.Id,
                        Price = new Price { Amount = 15.50m, Distance = 3.2 }
                    },
                    new Order
                    {
                        Description = "Large Pizza and drinks",
                        PickupAddress = "789 Pizza Place",
                        DeliveryAddress = "456 Home Ave",
                        Status = OrderStatus.Pending,
                        CustomerId = customer1.Id,
                        Price = new Price { Amount = 8.00m, Distance = 1.5 }
                    },
                    new Order
                    {
                        Description = "Electronics package",
                        PickupAddress = "Tech Shop Central",
                        DeliveryAddress = "101 Work Blvd",
                        Status = OrderStatus.Pending,
                        CustomerId = customer2.Id,
                        Price = new Price { Amount = 25.00m, Distance = 12.0 }
                    },
                    new Order
                    {
                        Description = "Flower bouquet",
                        PickupAddress = "The Flower Shop",
                        DeliveryAddress = "777 Romantic Rd",
                        Status = OrderStatus.Accepted,
                        CustomerId = customer2.Id,
                        DeliveryManId = delivery1.Id,
                        AcceptedOn = DateTime.UtcNow.AddMinutes(-15),
                        Price = new Price { Amount = 12.00m, Distance = 5.5 }
                    },
                    new Order
                    {
                        Description = "Office supplies",
                        PickupAddress = "Stationery World",
                        DeliveryAddress = "101 Work Blvd",
                        Status = OrderStatus.Delivered,
                        CustomerId = customer2.Id,
                        DeliveryManId = delivery1.Id,
                        AcceptedOn = DateTime.UtcNow.AddMinutes(-45),
                        Price = new Price { Amount = 10.00m, Distance = 2.8 }
                    }
                };

                dbContext.Orders.AddRange(orders);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
