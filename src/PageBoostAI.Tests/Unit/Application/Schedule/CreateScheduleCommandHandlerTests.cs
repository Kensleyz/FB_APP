using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Features.Schedule.Commands;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Tests.Unit.Application.Schedule;

public class CreateScheduleCommandHandlerTests
{
    private readonly IFacebookPageRepository _facebookPageRepository = Substitute.For<IFacebookPageRepository>();
    private readonly IContentScheduleRepository _contentScheduleRepository = Substitute.For<IContentScheduleRepository>();
    private readonly CreateScheduleCommandHandler _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid PageId = Guid.NewGuid();
    private static readonly DateTime FutureDate = DateTime.UtcNow.AddDays(1);

    public CreateScheduleCommandHandlerTests()
    {
        _sut = new CreateScheduleCommandHandler(_facebookPageRepository, _contentScheduleRepository);
    }

    private FacebookPage MakePage(Guid? userId = null)
    {
        var page = new FacebookPage(userId ?? UserId, "fb-page-id-123", "Test Page", "access-token");
        _facebookPageRepository.GetByIdAsync(PageId, Arg.Any<CancellationToken>()).Returns(page);
        return page;
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesScheduleAndReturnsDto()
    {
        // Arrange
        MakePage();
        _contentScheduleRepository.CountByPageIdAndDateAsync(PageId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(0);
        _contentScheduleRepository.AddAsync(Arg.Any<ContentSchedule>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<ContentSchedule>());

        var command = new CreateScheduleCommand(UserId, PageId, "Check out our new products!", FutureDate, null, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Content.Should().Be("Check out our new products!");
        result.Data.PageName.Should().Be("Test Page");
        result.Data.Status.Should().Be("Scheduled");
    }

    [Fact]
    public async Task Handle_PageNotFound_ReturnsFailure()
    {
        // Arrange
        _facebookPageRepository.GetByIdAsync(PageId, Arg.Any<CancellationToken>())
            .Returns((FacebookPage?)null);

        var command = new CreateScheduleCommand(UserId, PageId, "Post content", FutureDate, null, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Page not found"));
    }

    [Fact]
    public async Task Handle_PageBelongsToDifferentUser_ReturnsFailure()
    {
        // Arrange
        MakePage(Guid.NewGuid()); // different user

        var command = new CreateScheduleCommand(UserId, PageId, "Post content", FutureDate, null, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Page not found"));
    }

    [Fact]
    public async Task Handle_DailyLimitReached_ReturnsFailure()
    {
        // Arrange
        MakePage();
        _contentScheduleRepository.CountByPageIdAndDateAsync(PageId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(4); // max 4/day

        var command = new CreateScheduleCommand(UserId, PageId, "Post content", FutureDate, null, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Maximum of 4 posts per day"));
    }

    [Fact]
    public async Task Handle_WithHashtags_ParsesAndIncludesInSchedule()
    {
        // Arrange
        MakePage();
        _contentScheduleRepository.CountByPageIdAndDateAsync(PageId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(0);
        ContentSchedule? saved = null;
        _contentScheduleRepository.AddAsync(Arg.Do<ContentSchedule>(s => saved = s), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<ContentSchedule>());

        var command = new CreateScheduleCommand(UserId, PageId, "Post content", FutureDate, null, "spazashop,sale,deals", null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        saved!.Hashtags.Should().HaveCount(3);
        saved.Hashtags.Should().Contain("spazashop");
        saved.Hashtags.Should().Contain("sale");
        saved.Hashtags.Should().Contain("deals");
    }

    [Fact]
    public async Task Handle_ValidRequest_PersistsSchedule()
    {
        // Arrange
        MakePage();
        _contentScheduleRepository.CountByPageIdAndDateAsync(PageId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(0);
        _contentScheduleRepository.AddAsync(Arg.Any<ContentSchedule>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<ContentSchedule>());

        var command = new CreateScheduleCommand(UserId, PageId, "Post content", FutureDate, null, null, null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _contentScheduleRepository.Received(1).AddAsync(Arg.Any<ContentSchedule>(), Arg.Any<CancellationToken>());
    }
}
