using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<Result<bool>>;
