using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement forgot password logic
        await Task.CompletedTask;
        return Result.Success();
    }
}
