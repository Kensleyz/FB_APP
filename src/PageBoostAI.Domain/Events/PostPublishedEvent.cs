namespace PageBoostAI.Domain.Events;

public sealed class PostPublishedEvent : DomainEvent
{
    public Guid ContentScheduleId { get; }
    public Guid PageId { get; }
    public string FacebookPostId { get; }

    public PostPublishedEvent(Guid contentScheduleId, Guid pageId, string facebookPostId)
    {
        ContentScheduleId = contentScheduleId;
        PageId = pageId;
        FacebookPostId = facebookPostId;
    }
}
