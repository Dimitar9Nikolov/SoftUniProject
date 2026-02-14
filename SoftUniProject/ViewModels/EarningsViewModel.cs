namespace SoftUniProject.ViewModels;

public class EarningsViewModel
{
    public decimal TotalEarnings { get; set; }
    public IEnumerable<OrderListViewModel> CompletedDeliveries { get; set; } = new List<OrderListViewModel>();
}
