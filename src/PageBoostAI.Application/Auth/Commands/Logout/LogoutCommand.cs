using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;
