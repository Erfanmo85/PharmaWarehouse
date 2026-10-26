using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Repository;

namespace PharmaWarehouse.Application.Core.Queries.Reports;

public record ExpiringBatchDto(
    int BatchId, int DrugId, string DrugName, string BatchNumber,
    DateOnly ExpiryDate, int Quantity, int DaysLeft, bool IsExpired);

public record GetExpiringBatchesQuery(int Days = 30) : IRequest<List<ExpiringBatchDto>>;

public class GetExpiringBatchesValidator : AbstractValidator<GetExpiringBatchesQuery>
{
    public GetExpiringBatchesValidator()
    {
        RuleFor(x => x.Days).InclusiveBetween(1, 365);
    }
}

public class GetExpiringBatchesHandler : IRequestHandler<GetExpiringBatchesQuery, List<ExpiringBatchDto>>
{
    private readonly IBatchRepository _batches;

    public GetExpiringBatchesHandler(IBatchRepository batches)
    {
        _batches = batches;
    }

    public async Task<List<ExpiringBatchDto>> Handle(GetExpiringBatchesQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var until = today.AddDays(request.Days);

        var batches = await _batches.GetExpiringAsync(until, ct);

        return batches
            .Select(b => new ExpiringBatchDto(
                b.Id, b.DrugId, b.Drug.Name, b.BatchNumber, b.ExpiryDate, b.Quantity,
                b.ExpiryDate.DayNumber - today.DayNumber,   // منفی یعنی قبلاً منقضی شده
                b.IsExpired(today)))
            .ToList();
    }
}