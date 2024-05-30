using Common.DTOs.User.Request;
using Domain.Entities;
using Logging;
using Microsoft.AspNetCore.Identity;
using Services.Services;
using Tests.Mocks;
using Xunit.Abstractions;

namespace Tests.ServiceTests;

public class AuthenticationServiceTests : TestBase
{
    #region Setup
    private readonly ITestOutputHelper _testOutputHelper;
    private UserManager<User> _mockUserManager = null!;
    private AuthenticationService _authenticationService = null!;

    public AuthenticationServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Logger = new ConsoleLogger();
        
        MockSetup();
    }

    private void MockSetup()
    {
        _mockUserManager = UserManagerMock.GetMockUserManager();
        
        _authenticationService = new AuthenticationService(_mockUserManager, Configuration, Logger);
    }
    
    #endregion
    
    #region Tests
    [Fact]
    public async Task RegisterUser_WithRightCredentials_ReturnsTrue()
    {
        var userCreateModel = new UserCreateModel(
            "testUser", 
            "testPassword123123",
            "test@mail.com", 
            null
            );
        
        var res = await _authenticationService.RegisterUser(userCreateModel);
        
        Assert.True(res.Succeeded);
    }
    
    [Fact]
    public async Task Validator_WhenCalled_Validates()
    {
        var userLoginModel = new UserLoginModel("testUser", "testPassword123123");
        
        var res = await _authenticationService.ValidateUser(userLoginModel);
        
        Assert.True(res);
    }

    [Fact]
    public async Task GenerateJwtToken_WhenCalled_ReturnsTokenDto()
    {
        var userLoginModel = new UserLoginModel("testUser", "testPassword123123");
        
        var validateUser = await _authenticationService.ValidateUser(userLoginModel);
        
        Assert.True(validateUser);

        var res = await _authenticationService.GenerateJwtToken(true);

        Assert.NotNull(res);
        Assert.NotNull(res.AccessToken);
        Assert.NotNull(res.RefreshToken);

        _testOutputHelper.WriteLine("{0}: {1},", "JWT token", res.AccessToken);
        _testOutputHelper.WriteLine("{0}: {1},", "Refresh token", res.RefreshToken);
    }
    #endregion
}