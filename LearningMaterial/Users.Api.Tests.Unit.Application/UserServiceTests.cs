using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;
using Xunit;

namespace Users.Api.Tests.Unit.Application;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILoggerAdapter<UserService> _logger = Substitute.For<ILoggerAdapter<UserService>>();

    public UserServiceTests()
    {
        _sut = new UserService(_userRepository, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenSomeUsersExist()
    {
        // Arrange
        var nickChapsas = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Nick Chapsas"
        };
        var expectedUsers = new[]
        {
            nickChapsas
        };
        _userRepository.GetAllAsync().Returns(expectedUsers);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        //result.Single().Should().BeEquivalentTo(nickChapsas);
        result.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
    {
        // Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        // Act
        await _sut.GetAllAsync();

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Retrieving all users"));
        _logger.Received(1).LogInformation(Arg.Is("All users retrieved in {0}ms"), Arg.Any<long>());
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        // Arrange
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.GetAllAsync()
            .Throws(sqliteException);

        // Act
        var requestAction = async () => await _sut.GetAllAsync();

        // Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>().WithMessage("Something went wrong");
        _logger.Received(1).LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while retrieving all users"));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new User()
        {
            Id = Guid.NewGuid(),
            FullName = "David"
        };
        _userRepository.GetByIdAsync(existingUser.Id).Returns(existingUser);

        // Act
        var result = await _sut.GetByIdAsync(existingUser.Id);

        // Assert
        result.Should().BeEquivalentTo(existingUser);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserNotExists()
    {
        // Arrange
        // arrange a response for null for any guid

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogMessage_WhenInvoked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId).ReturnsNull();

        // Act
        await _sut.GetByIdAsync(userId);

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Retrieving user with id: {0}"), Arg.Is(userId));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} retrieved in {1}ms"), Arg.Is(userId), Arg.Any<long>());
    }

    [Fact]
    public async Task GetTaskAsync_ShouldLogMessage_WhenExpectionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var exception = new Exception("Something is wrong");
        _userRepository.GetByIdAsync(userId).Throws(exception);

        // Act
        var requestAction = async () => await _sut.GetByIdAsync(userId);

        // Assert
        await requestAction.Should().ThrowAsync<Exception>().WithMessage("Something is wrong");
        _logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Something went wrong while retrieving user with id {0}"), Arg.Is(userId));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenDetailsAreValid()
    {
        // Arrange
        var user = new User()
        {
            Id = Guid.NewGuid(),
            FullName = "David"
        };
        _userRepository.CreateAsync(user).Returns(true);

        // Act
        var result = await _sut.CreateAsync(user);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessage_WhenInvoked()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Nick Chapsas"
        };
        _userRepository.CreateAsync(user).Returns(true);

        // Act
        await _sut.CreateAsync(user);

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Creating user with id {0} and name: {1}"), Arg.Is(user.Id), Arg.Is(user.FullName));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} created in {1}ms"), Arg.Is(user.Id), Arg.Any<long>());
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "David"
        };
        var exception = new Exception("Something is wrong");
        _userRepository.CreateAsync(user).Throws(exception);

        // Act
        var requestAction = async () => await _sut.CreateAsync(user);

        // Assert
        await requestAction.Should().ThrowAsync<Exception>().WithMessage("Something is wrong");
        _logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Something went wrong while creating a user"));
    }
}
