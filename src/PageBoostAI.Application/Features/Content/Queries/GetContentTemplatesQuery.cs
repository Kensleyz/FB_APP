using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Content.Queries;

public record GetContentTemplatesQuery(Guid UserId) : IRequest<Result<List<ContentTemplateDto>>>;

public class GetContentTemplatesQueryHandler : IRequestHandler<GetContentTemplatesQuery, Result<List<ContentTemplateDto>>>
{
    private static readonly List<ContentTemplateDto> Templates =
    [
        new("promo-general",       "Promotional Post",       "Drive sales and awareness for your product or service.",       "General",        "🔥 {BusinessName} Special! {Offer}. Visit us today. #Sale #SpecialOffer"),
        new("promo-spaza",         "Spaza Shop Special",     "Weekend deals for spaza shop owners.",                         "SpazaShop",      "🛒 Weekend special at {BusinessName}! Stock up on your favourites. #Spaza #Deals"),
        new("promo-restaurant",    "Restaurant Daily Deal",  "Promote today's special or menu item.",                        "Restaurant",     "🍽️ Today's special at {BusinessName}: {Dish}. Come taste the difference! #Food #Eat"),
        new("event-church",        "Church Event",           "Announce a church gathering, service, or event.",              "Church",         "🙏 Join us at {BusinessName} this Sunday at {Time}. All are welcome! #Church #Faith"),
        new("promo-salon",         "Salon Promotion",        "Showcase your hair or beauty services.",                       "HairSalon",      "💇 Look amazing with {BusinessName}! Book your appointment today. #Hair #Beauty"),
        new("promo-gym",           "Gym Membership Drive",   "Attract new members with a special offer.",                    "Gym",            "💪 Join {BusinessName} and reach your fitness goals! Special rates this month. #Fitness #Gym"),
        new("engagement-question", "Engagement Question",    "Ask a question to boost post engagement.",                     "General",        "We want to hear from you! What do you love most about {BusinessName}? 👇 Comment below!"),
        new("community-update",    "Community Update",       "Share news or updates with your community.",                   "General",        "📢 Exciting update from {BusinessName}! {Update}. Stay tuned for more. #Community"),
        new("event-taxi",          "Taxi Association Notice","Communicate route or schedule updates.",                       "TaxiAssociation","🚌 {BusinessName} notice: {Update}. Safe travels to all our passengers. #Taxi #Transport"),
        new("promo-funeral",       "Funeral Parlour Notice", "Communicate services with dignity and care.",                  "FuneralParlor",  "🕊️ {BusinessName} offers compassionate funeral services. Call us anytime. #Dignified #Care"),
    ];

    public Task<Result<List<ContentTemplateDto>>> Handle(GetContentTemplatesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<List<ContentTemplateDto>>.Success(Templates));
    }
}
