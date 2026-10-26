using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Application.Core.Commands.Stock;

public record WriteOffBatchCommand(int BatchId, string? Note) : IRequest<int>;

public class WriteOffBatchValidator : AbstractValidator<WriteOffBatchCommand>
{
    public WriteOffBatchValidator()
    {
        RuleFor(x => x.BatchId).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(200);
    }
}

public class WriteOffBatchHandler : IRequestHandler<WriteOffBatchCommand, int>
{
    private readonly IBatchRepository _batches;
    private readonly IUnitOfWork _uow;

    public WriteOffBatchHandler(IBatchRepository batches, IUnitOfWork uow)
    {
        _batches = batches;
        _uow = uow;
    }

    public async Task<int> Handle(WriteOffBatchCommand request, CancellationToken ct)
    {
        var batch = await _batches.GetByIdAsync(request.BatchId, ct)
                    ?? throw new NotFoundException(nameof(Batch), request.BatchId);

        if (batch.Quantity == 0)
            throw new ConflictException("این بچ موجودی ندارد.");

        var amount = batch.Quantity;

        batch.Remove(amount);
        batch.Movements.Add(new StockMovement
        {
            Type = MovementType.Waste,
            Quantity = amount,
            Note = request.Note ?? "ضایعات"
        });

        await _uow.SaveChangesAsync(ct);
        return amount;   // تعداد ضایع‌شده
    }
}