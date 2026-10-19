using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Domain.Entities;

public class StockMovement : BaseEntity
{
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }

    public int BatchId { get; set; }
    public Batch Batch { get; set; } = null!;
}