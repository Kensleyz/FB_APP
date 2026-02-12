using MediatR;
using PageBoostAI.Application.Auth.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber) : IRequest<Result<AuthResponseDto>>;
