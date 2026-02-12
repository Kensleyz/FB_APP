using MediatR;
using PageBoostAI.Application.Auth.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;
