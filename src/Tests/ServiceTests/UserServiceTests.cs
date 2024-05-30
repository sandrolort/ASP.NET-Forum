using Common.DTOs.User.Request;
using Common.Exceptions;
using Common.Parameters;
using Services.Contracts.Contracts;
using Tests.Mocks;
using Xunit.Abstractions;

namespace Tests.ServiceTests;

public class UserServiceTests : TestBase
{
    #region Setup

    private IUserService _userService = null!;
    private ServiceMock _serviceMock = null!;
    
    public UserServiceTests(ITestOutputHelper testOutputHelper)
    {
        MockSetup();
    }
    
    private void MockSetup()
    {
        _serviceMock = new ServiceMock();
        _userService = _serviceMock.GetServiceManager().UserService;
    }

    #endregion
    
    #region Tests
    
    [Fact]
    public async Task GetUser_WhenUserExists_ShouldReturnUser()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        
        var result = await _userService.GetUserById(user.Id, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.UserName, result.UserName);
    }
    
    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => _userService.GetUserById("NonExistentUserId", CancellationToken.None));
    }
    
    [Fact]
    public async Task GetUserByEmail_WhenUserExists_ShouldReturnUser()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        
        var result = await _userService.GetUserByEmail(user.Email, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.UserName, result.UserName);
    }
    
    [Fact]
    public async Task GetUserByEmail_WhenUserDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => _userService.GetUserByEmail("NonExistentUserEmail", CancellationToken.None));
    }
    
    [Fact]
    public async Task GetAllUsers_WhenUsersExist_ShouldReturnUsers()
    {
        var result = await _userService.GetAllUsers(CancellationToken.None, new RequestParameters());
        
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task GetUserComments_WhenUserExists_ShouldReturnComments()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        
        var result = await _userService.GetUserComments(user.Id, CancellationToken.None, new RequestParameters());
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetUserTopics_WhenUserExists_ShouldReturnTopics()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        
        var result = await _userService.GetUserTopics(user.Id, CancellationToken.None, new RequestParameters());
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task UpdateUser_WhenUserExists_ShouldUpdateUser()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        
        var userUpdateModel = new UserUpdateModel
        (
            "Test",
            "Test",
            "Test"
        );
        
        var result = await _userService.UpdateUser(user.Id, userUpdateModel, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal("Test", result.UserName);
        Assert.Equal("Test", result.Email);
        Assert.Equal("Test", result.ProfilePicUrl);
    }
    
    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        var userUpdateModel = new UserUpdateModel
        (
            "Test",
            "Test",
            "Test"
        );
        
        await Assert.ThrowsAsync<NotFound>(() => _userService.UpdateUser("NonExistentUserId", userUpdateModel, CancellationToken.None));
    }
    
    #endregion
}