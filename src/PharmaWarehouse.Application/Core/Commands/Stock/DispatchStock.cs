using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Application.Core.Commands.Stock;

public record DispatchedItemDto(int BatchId, string BatchNumber, DateOnly ExpiryDate, int Quantity);

public record DispatchStockCommand(int DrugId, int Quantity, string? Note)
    : IRequest<List<DispatchedItemDto>>;

public class DispatchStockValidator : AbstractValidator<DispatchStockCommand>
{
    public DispatchStockValidator()
    {
        RuleFor(x => x.DrugId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(200);
    }
}

public class DispatchStockHandler : IRequestHandler<DispatchStockCommand, List<DispatchedItemDto>>
{
    private readonly IDrugRepository _drugs;
    private readonly IBatchRepository _batches;
    private readonly IUnitOfWork _uow;

    public DispatchStockHandler(IDrugRepository drugs, IBatchRepository batches, IUnitOfWork uow)
    {
        _drugs = drugs;
        _batches = batches;
        _uow = uow;
    }

    public async Task<List<DispatchedItemDto>> Handle(DispatchStockCommand request, CancellationToken ct)
    {
        var drug = await _drugs.GetByIdAsync(request.DrugId, ct)
                   ?? throw new NotFoundException(nameof(Drug), request.DrugId);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var batches = await _batches.GetAvailableForDispatchAsync(drug.Id, today, ct);

        var available = batches.Sum(b => b.Quantity);
        if (available < request.Quantity)
            throw new ConflictException(
                $"موجودی قابل عرضه کافی نیست. درخواستی: {request.Quantity}، موجود (غیرمنقضی): {available}");

        var remaining = request.Quantity;
        var result = new List<DispatchedItemDto>();

        foreach (var batch in batches)
        {
            if (remaining == 0) break;

            var take = Math.Min(batch.Quantity, remaining);

            batch.Remove(take);
            batch.Movements.Add(new StockMovement
            {
                Type = MovementType.Outbound,
                Quantity = take,
                Note = request.Note
            });

            result.Add(new DispatchedItemDto(batch.Id, batch.BatchNumber, batch.ExpiryDate, take));
            remaining -= take;
        }

        await _uow.SaveChangesAsync(ct);   // همه تغییرات در یک تراکنش
        return result;
    }
}