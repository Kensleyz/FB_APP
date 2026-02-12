using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result<bool>>;
