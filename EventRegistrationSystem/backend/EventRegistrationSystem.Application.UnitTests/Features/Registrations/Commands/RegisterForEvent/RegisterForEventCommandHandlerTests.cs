using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Application.Exceptions;
using EventRegistrationSystem.Application.Features.Registrations.Commands.RegisterForEvent;
using EventRegistrationSystem.Application.UnitTests.Helpers;
using EventRegistrationSystem.Domain.Entities;
using EventRegistrationSystem.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventRegistrationSystem.Application.UnitTests.Features.Registrations.Commands.RegisterForEvent;

public class RegisterForEventCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _contextMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly RegisterForEventCommandHandler _handler;

    public RegisterForEventCommandHandlerTests()
    {
        _contextMock = new Mock<IAppDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new RegisterForEventCommandHandler(_contextMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ThrowsEventNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId.ToString());
        
        var command = new RegisterForEventCommand(Guid.NewGuid());
        var events = new List<Event>().AsQueryable();
        var eventsDbSetMock = MockDbSetHelper.CreateMockDbSet(events);
        _contextMock.Setup(x => x.Events).Returns(eventsDbSetMock.Object);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyRegistered_ThrowsRegistrationAlreadyExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId.ToString());
        
        var command = new RegisterForEventCommand(eventId);
        
        var ev = new Event 
        { 
            Id = eventId, 
            Capacity = 10, 
            StartDate = DateTime.UtcNow.AddDays(1) 
        };
        var events = new List<Event> { ev }.AsQueryable();
        var eventsDbSetMock = MockDbSetHelper.CreateMockDbSet(events);
        _contextMock.Setup(x => x.Events).Returns(eventsDbSetMock.Object);

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            UserId = userId,
            Status = RegistrationStatus.Registered
        };
        var registrations = new List<Registration> { registration }.AsQueryable();
        var registrationsDbSetMock = MockDbSetHelper.CreateMockDbSet(registrations);
        _contextMock.Setup(x => x.Registrations).Returns(registrationsDbSetMock.Object);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<RegistrationAlreadyExistsException>();
    }

    [Fact]
    public async Task Handle_CapacityExceeded_ThrowsEventCapacityExceededException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId.ToString());
        
        var command = new RegisterForEventCommand(eventId);
        
        var ev = new Event 
        { 
            Id = eventId, 
            Capacity = 0, 
            StartDate = DateTime.UtcNow.AddDays(1) 
        };
        var events = new List<Event> { ev }.AsQueryable();
        var eventsDbSetMock = MockDbSetHelper.CreateMockDbSet(events);
        _contextMock.Setup(x => x.Events).Returns(eventsDbSetMock.Object);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EventCapacityExceededException>();
    }

    [Fact]
    public async Task Handle_EventStarted_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId.ToString());
        
        var command = new RegisterForEventCommand(eventId);
        
        var ev = new Event 
        { 
            Id = eventId, 
            Capacity = 10, 
            StartDate = DateTime.UtcNow.AddDays(-1) 
        };
        var events = new List<Event> { ev }.AsQueryable();
        var eventsDbSetMock = MockDbSetHelper.CreateMockDbSet(events);
        _contextMock.Setup(x => x.Events).Returns(eventsDbSetMock.Object);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Cannot register for an event that has already started.");
    }

    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsRegistrationIdAndDecreasesCapacity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId.ToString());
        
        var command = new RegisterForEventCommand(eventId);
        
        var initialCapacity = 10;
        var ev = new Event 
        { 
            Id = eventId, 
            Capacity = initialCapacity, 
            StartDate = DateTime.UtcNow.AddDays(1) 
        };
        var events = new List<Event> { ev }.AsQueryable();
        var eventsDbSetMock = MockDbSetHelper.CreateMockDbSet(events);
        _contextMock.Setup(x => x.Events).Returns(eventsDbSetMock.Object);

        var registrations = new List<Registration>().AsQueryable();
        var registrationsDbSetMock = MockDbSetHelper.CreateMockDbSet(registrations);
        _contextMock.Setup(x => x.Registrations).Returns(registrationsDbSetMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        ev.Capacity.Should().Be(initialCapacity - 1);
        registrationsDbSetMock.Verify(x => x.Add(It.Is<Registration>(r => r.EventId == eventId && r.UserId == userId)), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
