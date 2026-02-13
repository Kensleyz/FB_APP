using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record VerifyEmailCommand(string Token) : IRequest<Result>;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement email verification logic
        await Task.CompletedTask;
        return Result.Failure("Not implemented yet");
    }
}
