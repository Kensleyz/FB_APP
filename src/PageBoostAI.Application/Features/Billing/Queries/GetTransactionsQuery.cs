using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Billing.Queries;

public record GetTransactionsQuery(Guid UserId) : IRequest<Result<List<TransactionDto>>>;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, Result<List<TransactionDto>>>
{
    private readonly IPaymentTransactionRepository _transactionRepository;

    public GetTransactionsQueryHandler(IPaymentTransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<List<TransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = transactions.Select(t => new TransactionDto(
            t.Id,
            t.Amount.Amount,
            t.Currency,
            t.Status.ToString(),
            t.PaymentMethod,
            t.TransactionType,
            t.CreatedAt,
            t.CompletedAt)).ToList();

        return Result<List<TransactionDto>>.Success(dtos);
    }
}
