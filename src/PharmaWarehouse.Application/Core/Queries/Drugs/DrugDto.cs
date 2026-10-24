using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Core.Queries.Drugs;

public record DrugDto(
    int Id, string Name, string Code, string DosageForm,
    int MinStockLevel, int CategoryId, string CategoryName,
    int TotalStock, bool IsLowStock)
{
    public static DrugDto FromEntity(Drug d) => new(
        d.Id, d.Name, d.Code, d.DosageForm.ToString(),
        d.MinStockLevel, d.CategoryId, d.Category.Name,
        d.TotalStock, d.IsLowStock);
}