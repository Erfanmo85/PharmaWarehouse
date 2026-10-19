using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Domain.Entities;

public class Drug : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DosageForm DosageForm { get; set; }
    public int MinStockLevel { get; set; }   // حداقل موجودی برای هشدار کمبود

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public int TotalStock => Batches.Sum(b => b.Quantity);
    public bool IsLowStock => TotalStock <= MinStockLevel;
}