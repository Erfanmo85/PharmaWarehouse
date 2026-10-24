using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Application.Core.Commands.Batches;

public record CreateBatchCommand(
    int DrugId, string BatchNumber, DateOnly ManufactureDate,
    DateOnly ExpiryDate, int Quantity, decimal PurchasePrice) : IRequest<int>;

public class CreateBatchValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchValidator()
    {
        RuleFor(x => x.DrugId).GreaterThan(0);
        RuleFor(x => x.BatchNumber).NotEmpty().MaximumLength(50);

        RuleFor(x => x.ManufactureDate)
            .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("تاریخ تولید نمی‌تواند در آینده باشد.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.ManufactureDate)
            .WithMessage("تاریخ انقضا باید بعد از تاریخ تولید باشد.")
            .Must(d => d > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("این بچ منقضی شده و نمی‌توان آن را ثبت کرد.");

        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
    }
}

public class CreateBatchHandler : IRequestHandler<CreateBatchCommand, int>
{
    private readonly IDrugRepository _drugs;
    private readonly IBatchRepository _batches;
    private readonly IUnitOfWork _uow;

    public CreateBatchHandler(IDrugRepository drugs, IBatchRepository batches, IUnitOfWork uow)
    {
        _drugs = drugs;
        _batches = batches;
        _uow = uow;
    }

    public async Task<int> Handle(CreateBatchCommand request, CancellationToken ct)
    {
        var drug = await _drugs.GetByIdAsync(request.DrugId, ct)
                   ?? throw new NotFoundException(nameof(Drug), request.DrugId);

        if (await _batches.NumberExistsAsync(drug.Id, request.BatchNumber, ct))
            throw new ConflictException($"بچ «{request.BatchNumber}» برای این دارو قبلاً ثبت شده است.");

        var batch = new Batch
        {
            DrugId = drug.Id,
            BatchNumber = request.BatchNumber,
            ManufactureDate = request.ManufactureDate,
            ExpiryDate = request.ExpiryDate,
            Quantity = request.Quantity,
            PurchasePrice = request.PurchasePrice
        };

        // هر تغییر موجودی باید یک ردیف در دفتر ثبت داشته باشد
        batch.Movements.Add(new StockMovement
        {
            Type = MovementType.Inbound,
            Quantity = request.Quantity,
            Note = "ثبت اولیه بچ"
        });

        await _batches.AddAsync(batch, ct);
        await _uow.SaveChangesAsync(ct);

        return batch.Id;
    }
}