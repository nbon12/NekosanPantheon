using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using HelloWorldFunctionApp;
using HelloWorldFunctionApp.Data;
using HelloWorldFunctionApp.Models;
using HelloWorldFunctionApp.Tests.TestHelpers;

namespace HelloWorldFunctionApp.Tests;

public class HelloWorldFunctionTests
{
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<ILogger<HelloWorldFunction>> _loggerMock;
    private readonly DbContextOptions<AppDbContext> _options;

    public HelloWorldFunctionTests()
    {
        _loggerMock = new Mock<ILogger<HelloWorldFunction>>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(_loggerMock.Object);

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetUsers_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "GET", "");

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var testResponse = response as TestHttpResponseData;
        Assert.NotNull(testResponse);
        
        var responseBody = testResponse!.GetBody();
        var users = JsonSerializer.Deserialize<List<User>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task GetUsers_ReturnsAllUsers_WhenUsersExist()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        context.Users.Add(new User { Id = "1", Name = "John Doe", Email = "john@example.com" });
        context.Users.Add(new User { Id = "2", Name = "Jane Smith", Email = "jane@example.com" });
        await context.SaveChangesAsync();

        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "GET", "");

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var testResponse = response as TestHttpResponseData;
        Assert.NotNull(testResponse);
        
        var responseBody = testResponse!.GetBody();
        var users = JsonSerializer.Deserialize<List<User>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(users);
        Assert.Equal(2, users.Count);
        Assert.Contains(users, u => u.Name == "John Doe");
        Assert.Contains(users, u => u.Name == "Jane Smith");
    }

    [Fact]
    public async Task AddUser_CreatesUser_WhenValidDataProvided()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        
        var user = new User { Name = "Test User", Email = "test@example.com" };
        var json = JsonSerializer.Serialize(user, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "POST", json);

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var testResponse = response as TestHttpResponseData;
        Assert.NotNull(testResponse);
        
        var responseBody = testResponse!.GetBody();
        var createdUser = JsonSerializer.Deserialize<User>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(createdUser);
        Assert.Equal("Test User", createdUser.Name);
        Assert.Equal("test@example.com", createdUser.Email);
        Assert.False(string.IsNullOrEmpty(createdUser.Id));

        // Verify user was saved to database
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == createdUser.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("Test User", savedUser.Name);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenRequestBodyIsEmpty()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "POST", "");

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var testResponse = response as TestHttpResponseData;
        Assert.NotNull(testResponse);
        
        var responseBody = testResponse!.GetBody();
        Assert.Contains("Request body is required", responseBody);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenNameIsMissing()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        
        var user = new { Email = "test@example.com" };
        var json = JsonSerializer.Serialize(user);
        
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "POST", json);

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var testResponse = response as TestHttpResponseData;
        Assert.NotNull(testResponse);
        
        var responseBody = testResponse!.GetBody();
        Assert.Contains("Name and Email are required", responseBody);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenEmailIsMissing()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        
        var user = new { Name = "Test User" };
        var json = JsonSerializer.Serialize(user);
        
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "POST", json);

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var testResponse = response as TestHttpResponseData;
        Assert.NotNull(testResponse);
        
        var responseBody = testResponse!.GetBody();
        Assert.Contains("Name and Email are required", responseBody);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenInvalidJsonProvided()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "POST", "{ invalid json }");

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Run_ReturnsMethodNotAllowed_WhenUnsupportedMethodUsed()
    {
        // Arrange
        using var context = new AppDbContext(_options);
        var function = new HelloWorldFunction(_loggerFactoryMock.Object, context);
        var functionContext = new TestFunctionContext();
        var requestData = new TestHttpRequestData(functionContext, "DELETE", "");

        // Act
        var response = await function.Run(requestData, functionContext);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }
}
