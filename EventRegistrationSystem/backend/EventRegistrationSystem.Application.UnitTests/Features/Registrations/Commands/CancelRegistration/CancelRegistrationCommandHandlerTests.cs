using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Application.Exceptions;
using EventRegistrationSystem.Application.Features.Registrations.Commands.CancelRegistration;
using EventRegistrationSystem.Application.UnitTests.Helpers;
using EventRegistrationSystem.Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace EventRegistrationSystem.Application.UnitTests.Features.Registrations.Commands.CancelRegistration;

public class CancelRegistrationCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _contextMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CancelRegistrationCommandHandler _handler;

    public CancelRegistrationCommandHandlerTests()
    {
        _contextMock = new Mock<IAppDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new CancelRegistrationCommandHandler(_contextMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId.ToString());

        var command = new CancelRegistrationCommand(Guid.NewGuid());
        var registrations = new List<Registration>().AsQueryable();
        var registrationsDbSetMock = MockDbSetHelper.CreateMockDbSet(registrations);
        _contextMock.Setup(x => x.Registrations).Returns(registrationsDbSetMock.Object);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ForbiddenOperation_ThrowsForbiddenOperationException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var registrationId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId.ToString());
        _currentUserServiceMock.Setup(x => x.IsAdmin).Returns(false);

        var command = new CancelRegistrationCommand(registrationId);

        var registration = new Registration
        {
            Id = registrationId,
            UserId = otherUserId,
            Event = new Event { Id = Guid.NewGuid(), Capacity = 10 }
        };
        var registrations = new List<Registration> { registration }.AsQueryable();
        var registrationsDbSetMock = MockDbSetHelper.CreateMockDbSet(registrations);
        _contextMock.Setup(x => x.Registrations).Returns(registrationsDbSetMock.Object);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenOperationException>()
            .WithMessage("You are not allowed to cancel someone else's registration.");
    }

    [Fact]
    public async Task Handle_Cancel_And_IncreaseCapacity_When_Successful()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var registrationId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId.ToString());
        _currentUserServiceMock.Setup(x => x.IsAdmin).Returns(false);

        var command = new CancelRegistrationCommand(registrationId);

        var initialCapacity = 10;
        var ev = new Event { Id = Guid.NewGuid(), Capacity = initialCapacity };
        var registration = new Registration
        {
            Id = registrationId,
            UserId = currentUserId,
            Event = ev
        };
        var registrations = new List<Registration> { registration }.AsQueryable();
        var registrationsDbSetMock = MockDbSetHelper.CreateMockDbSet(registrations);
        _contextMock.Setup(x => x.Registrations).Returns(registrationsDbSetMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        ev.Capacity.Should().Be(initialCapacity + 1);
        registrationsDbSetMock.Verify(x => x.Remove(registration), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
