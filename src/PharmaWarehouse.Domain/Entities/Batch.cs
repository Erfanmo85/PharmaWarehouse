namespace PharmaWarehouse.Domain.Entities;

public class Batch : BaseEntity
{
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ManufactureDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }

    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();

    public bool IsExpired(DateOnly today) => ExpiryDate < today;

    public void Add(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("مقدار باید بزرگتر از صفر باشد.");

        Quantity += amount;
    }

    public void Remove(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("مقدار باید بزرگتر از صفر باشد.");
        if (amount > Quantity)
            throw new InvalidOperationException("موجودی بچ کافی نیست.");

        Quantity -= amount;
    }
}