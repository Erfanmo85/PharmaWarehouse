using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Application.Core.Commands.Drugs;

public record UpdateDrugCommand(
    int Id, string Name, string Code, DosageForm DosageForm,
    int MinStockLevel, int CategoryId) : IRequest;

public class UpdateDrugValidator : AbstractValidator<UpdateDrugCommand>
{
    public UpdateDrugValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DosageForm).IsInEnum();
        RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class UpdateDrugHandler : IRequestHandler<UpdateDrugCommand>
{
    private readonly IDrugRepository _drugs;
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public UpdateDrugHandler(IDrugRepository drugs, ICategoryRepository categories, IUnitOfWork uow)
    {
        _drugs = drugs;
        _categories = categories;
        _uow = uow;
    }

    public async Task Handle(UpdateDrugCommand request, CancellationToken ct)
    {
        var drug = await _drugs.GetByIdAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Drug), request.Id);

        if (!await _categories.ExistsAsync(request.CategoryId, ct))
            throw new NotFoundException(nameof(Category), request.CategoryId);

        if (await _drugs.CodeExistsAsync(request.Code, request.Id, ct))
            throw new ConflictException($"دارویی با کد «{request.Code}» از قبل وجود دارد.");

        drug.Name = request.Name;
        drug.Code = request.Code;
        drug.DosageForm = request.DosageForm;
        drug.MinStockLevel = request.MinStockLevel;
        drug.CategoryId = request.CategoryId;

        await _uow.SaveChangesAsync(ct);   // EF خودش می‌فهمد چه چیزی عوض شده
    }
}