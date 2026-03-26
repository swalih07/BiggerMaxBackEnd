using Domain.Entities;
using Domain.Enums;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null;

    public int ShippingAddressId { get; set; }
    public ShippingAddress ShippingAddress { get; set; } = null!;

    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}