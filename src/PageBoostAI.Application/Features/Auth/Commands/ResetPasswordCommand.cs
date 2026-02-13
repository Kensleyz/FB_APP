using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement reset password logic
        await Task.CompletedTask;
        return Result.Failure("Not implemented yet");
    }
}
