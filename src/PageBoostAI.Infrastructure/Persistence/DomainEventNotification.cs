using MediatR;
using PageBoostAI.Domain.Events;

namespace PageBoostAI.Infrastructure.Persistence;

public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
