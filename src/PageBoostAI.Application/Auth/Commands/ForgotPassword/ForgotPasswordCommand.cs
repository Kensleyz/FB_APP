using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;
