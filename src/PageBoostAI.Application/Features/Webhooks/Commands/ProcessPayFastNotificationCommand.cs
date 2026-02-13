using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Webhooks.Commands;

public record ProcessPayFastNotificationCommand(PayFastNotificationDto Dto, string? SourceIp) : IRequest<Result>;

public class ProcessPayFastNotificationCommandHandler : IRequestHandler<ProcessPayFastNotificationCommand, Result>
{
    public async Task<Result> Handle(ProcessPayFastNotificationCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement PayFast notification processing logic
        await Task.CompletedTask;
        return Result.Failure("Not implemented yet");
    }
}
