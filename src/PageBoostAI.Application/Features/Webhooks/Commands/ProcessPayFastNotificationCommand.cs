using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Webhooks.Commands;

public record ProcessPayFastNotificationCommand(PayFastNotificationDto Dto, string? SourceIp) : IRequest<Result>;

public class ProcessPayFastNotificationCommandHandler : IRequestHandler<ProcessPayFastNotificationCommand, Result>
{
    private readonly IPayFastService _payFastService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public ProcessPayFastNotificationCommandHandler(
        IPayFastService payFastService,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _payFastService = payFastService;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(ProcessPayFastNotificationCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (!_payFastService.ValidateWebhook(BuildFormData(dto), dto.signature ?? string.Empty))
            return Result.Failure("Invalid PayFast signature.");

        if (!Guid.TryParse(dto.custom_str1, out var userId))
            return Result.Failure("Invalid user reference in notification.");

        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        if (subscription is null)
            return Result.Failure("No active subscription found for this user.");

        await ApplyStatusAsync(dto.payment_status?.ToUpperInvariant(), subscription, userId, cancellationToken);

        return Result.Success();
    }

    private async Task ApplyStatusAsync(string? status, Domain.Entities.Subscription subscription, Guid userId, CancellationToken cancellationToken)
    {
        switch (status)
        {
            case "CANCELLED":
                subscription.Cancel();
                await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
                await DowngradeUserToFreeAsync(userId, cancellationToken);
                break;

            case "FAILED":
                subscription.MarkPastDue();
                await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
                break;
        }
    }

    private async Task DowngradeUserToFreeAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null) return;
        user.UpdateSubscription(Domain.Enums.SubscriptionTier.Free);
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    private static Dictionary<string, string> BuildFormData(PayFastNotificationDto dto)
    {
        var data = new Dictionary<string, string>();
        if (dto.m_payment_id is not null)   data["m_payment_id"]   = dto.m_payment_id;
        if (dto.pf_payment_id is not null)  data["pf_payment_id"]  = dto.pf_payment_id;
        if (dto.payment_status is not null) data["payment_status"] = dto.payment_status;
        if (dto.item_name is not null)      data["item_name"]      = dto.item_name;
        if (dto.amount_gross is not null)   data["amount_gross"]   = dto.amount_gross;
        if (dto.amount_fee is not null)     data["amount_fee"]     = dto.amount_fee;
        if (dto.amount_net is not null)     data["amount_net"]     = dto.amount_net;
        if (dto.custom_str1 is not null)    data["custom_str1"]    = dto.custom_str1;
        return data;
    }
}
