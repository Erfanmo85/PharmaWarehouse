using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Common.Interfaces;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Core.Commands.Drugs;

public record DeleteDrugCommand(int Id) : IRequest;

public class DeleteDrugHandler : IRequestHandler<DeleteDrugCommand>
{
    private readonly IDrugRepository _drugs;
    private readonly IUnitOfWork _uow;

    public DeleteDrugHandler(IDrugRepository drugs, IUnitOfWork uow)
    {
        _drugs = drugs;
        _uow = uow;
    }

    public async Task Handle(DeleteDrugCommand request, CancellationToken ct)
    {
        var drug = await _drugs.GetByIdAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Drug), request.Id);

        _drugs.Remove(drug);
        await _uow.SaveChangesAsync(ct);
    }
}