using MediatR;
using PageBoostAI.Application.Billing.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Queries.GetTransactions;

public record GetTransactionsQuery : IRequest<Result<List<TransactionDto>>>;
