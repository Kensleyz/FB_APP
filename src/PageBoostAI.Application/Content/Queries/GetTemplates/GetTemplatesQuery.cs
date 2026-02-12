using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Application.Content.Queries.GetTemplates;

public record TemplatesDto(
    List<string> BusinessTypes,
    List<string> ToneOptions,
    List<string> PostTypes,
    List<string> Languages);

public record GetTemplatesQuery : IRequest<Result<TemplatesDto>>;

public class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, Result<TemplatesDto>>
{
    public Task<Result<TemplatesDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
    {
        var dto = new TemplatesDto(
            Enum.GetNames<BusinessType>().ToList(),
            Enum.GetNames<ToneOption>().ToList(),
            Enum.GetNames<PostType>().ToList(),
            new List<string> { "English", "Afrikaans", "Zulu", "Xhosa", "Sotho", "Tswana" });

        return Task.FromResult(Result<TemplatesDto>.Success(dto));
    }
}
