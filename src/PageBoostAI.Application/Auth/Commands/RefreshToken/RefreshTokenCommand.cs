using MediatR;
using PageBoostAI.Application.Auth.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;
