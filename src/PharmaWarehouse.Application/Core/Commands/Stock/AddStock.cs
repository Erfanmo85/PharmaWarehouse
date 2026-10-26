using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Application.Core.Commands.Stock;

public record AddStockCommand(int BatchId, int Quantity, string? Note) : IRequest<int>;

public class AddStockValidator : AbstractValidator<AddStockCommand>
{
    public AddStockValidator()
    {
        RuleFor(x => x.BatchId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(200);
    }
}

public class AddStockHandler : IRequestHandler<AddStockCommand, int>
{
    private readonly IBatchRepository _batches;
    private readonly IUnitOfWork _uow;

    public AddStockHandler(IBatchRepository batches, IUnitOfWork uow)
    {
        _batches = batches;
        _uow = uow;
    }

    public async Task<int> Handle(AddStockCommand request, CancellationToken ct)
    {
        var batch = await _batches.GetByIdAsync(request.BatchId, ct)
                    ?? throw new NotFoundException(nameof(Batch), request.BatchId);

        if (batch.IsExpired(DateOnly.FromDateTime(DateTime.UtcNow)))
            throw new ConflictException("این بچ منقضی شده و نمی‌توان به آن موجودی اضافه کرد.");

        batch.Add(request.Quantity);
        batch.Movements.Add(new StockMovement
        {
            Type = MovementType.Inbound,
            Quantity = request.Quantity,
            Note = request.Note
        });

        await _uow.SaveChangesAsync(ct);
        return batch.Quantity;   // موجودی جدید بچ
    }
}