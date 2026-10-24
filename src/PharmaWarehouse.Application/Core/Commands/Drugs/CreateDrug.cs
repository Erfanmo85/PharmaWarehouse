using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Application.Core.Commands.Drugs;

public record CreateDrugCommand(
    string Name, string Code, DosageForm DosageForm,
    int MinStockLevel, int CategoryId) : IRequest<int>;

public class CreateDrugValidator : AbstractValidator<CreateDrugCommand>
{
    public CreateDrugValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DosageForm).IsInEnum();
        RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class CreateDrugHandler : IRequestHandler<CreateDrugCommand, int>
{
    private readonly IDrugRepository _drugs;
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public CreateDrugHandler(IDrugRepository drugs, ICategoryRepository categories, IUnitOfWork uow)
    {
        _drugs = drugs;
        _categories = categories;
        _uow = uow;
    }

    public async Task<int> Handle(CreateDrugCommand request, CancellationToken ct)
    {
        if (!await _categories.ExistsAsync(request.CategoryId, ct))
            throw new NotFoundException(nameof(Category), request.CategoryId);

        if (await _drugs.CodeExistsAsync(request.Code, null, ct))
            throw new ConflictException($"دارویی با کد «{request.Code}» از قبل وجود دارد.");

        var drug = new Drug
        {
            Name = request.Name,
            Code = request.Code,
            DosageForm = request.DosageForm,
            MinStockLevel = request.MinStockLevel,
            CategoryId = request.CategoryId
        };

        await _drugs.AddAsync(drug, ct);
        await _uow.SaveChangesAsync(ct);

        return drug.Id;
    }
}