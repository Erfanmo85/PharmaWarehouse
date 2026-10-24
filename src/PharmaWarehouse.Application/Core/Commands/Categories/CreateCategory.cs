using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Core.Commands.Categories;

// ۱) درخواست: چه داده‌ای لازم است و چه چیزی برمی‌گردد (شناسه جدید)
public record CreateCategoryCommand(string Name) : IRequest<int>;

// ۲) قوانین اعتبارسنجی
public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

// ۳) کارمندی که کار را انجام می‌دهد
public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public CreateCategoryHandler(ICategoryRepository categories, IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (await _categories.NameExistsAsync(request.Name, ct))
            throw new ConflictException($"دسته‌بندی «{request.Name}» از قبل وجود دارد.");

        var category = new Category { Name = request.Name };

        await _categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

        return category.Id;
    }
}