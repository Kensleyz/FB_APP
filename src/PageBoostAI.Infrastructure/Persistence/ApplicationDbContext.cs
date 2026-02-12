using MediatR;
using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Common;
using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FacebookPage> FacebookPages => Set<FacebookPage>();
    public DbSet<ContentSchedule> ContentSchedules => Set<ContentSchedule>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<UsageMetrics> UsageMetrics => Set<UsageMetrics>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
        }

        // Collect domain events before saving
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events before save to prevent re-dispatching
        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after save
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent)!;
            await _mediator.Publish(notification, cancellationToken);
        }

        return result;
    }
}
