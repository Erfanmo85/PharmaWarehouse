namespace PharmaWarehouse.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Drug> Drugs { get; set; } = new List<Drug>();
}