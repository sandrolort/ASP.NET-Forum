using Common.DTOs.Ban.Request;
using Common.Exceptions;
using Common.Parameters;
using Services.Contracts.Contracts;
using Tests.Mocks;
using Xunit.Abstractions;

namespace Tests.ServiceTests;

public class BanServiceTests : TestBase
{
    #region Setup
    private readonly ITestOutputHelper _testOutputHelper;
    private ServiceMock _serviceMock = null!;
    private IBanService _banService = null!;

    public BanServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        MockSetup();
    }

    private void MockSetup()
    {
        UserManagerMock.Instance();
        _serviceMock = new ServiceMock();
        _banService = _serviceMock.GetServiceManager().BanService;
    }
    
    #endregion
    
    #region Tests

    [Fact]
    public async Task GetBanById_WhenBanExists_ShouldReturnBan()
    {
        var ban = _serviceMock.RandomDataGenerator.GeneratedBanData.First();
        
        var result = await _banService.GetBanById(ban.Id, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(ban.Id, result.Id);
        Assert.Equal(ban.UserId, result.UserId);
    }
    
    [Fact]
    public async Task GetBanById_WhenBanDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => _banService.GetBanById(uint.MaxValue, CancellationToken.None));
    }
    
    [Fact]
    public async Task GetBans_WhenBansExist_ShouldReturnBans()
    {
        var result = await _banService.GetAllBans(CancellationToken.None, new RequestParameters());
        
        _testOutputHelper.WriteLine(result.Count().ToString());
        
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task BanUser_WhenUserExists_ShouldBanUser()
    {
        var banCreateModel = new BanCreateModel
        (
            _serviceMock.RandomDataGenerator.GeneratedUserData.First(u=>!u.IsBanned).Id,
            "Test",
            DateTime.Now.AddDays(1)
        );

        _testOutputHelper.WriteLine(banCreateModel.ToString());

        var result = await _banService.CreateBan(banCreateModel, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(banCreateModel.UserId, result.UserId);
        Assert.Equal(banCreateModel.Reason, result.Reason);
        Assert.Equal(banCreateModel.BanEndDate, result.BanEndDate);
    }
    
    [Fact]
    public async Task BanUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        var banCreateModel = new BanCreateModel
        (
            "NonExistentUserId",
            "Test",
            DateTime.Now.AddDays(1)
        );

        await Assert.ThrowsAsync<NotFound>(() => _banService.CreateBan(banCreateModel, CancellationToken.None));
    }
    
    [Fact]
    public async Task BanUser_WhenUserIsAlreadyBanned_ShouldThrowException()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        var banCreateModel = new BanCreateModel
        (
            user.Id,
            "Test",
            DateTime.Now.AddDays(1)
        );

        if (!user.IsBanned) await _banService.CreateBan(banCreateModel, CancellationToken.None);
        await Assert.ThrowsAsync<Conflict>(() => _banService.CreateBan(banCreateModel, CancellationToken.None));
    }
    
    [Fact]
    public async Task UpdateBan_WhenBanExists_ShouldUpdateBan()
    {
        var ban = _serviceMock.RandomDataGenerator.GeneratedBanData.First();
        
        var banUpdateModel = new BanUpdateModel
        (
            DateTime.Now.AddDays(2),
            "Test"
        );

        var result = await _banService.UpdateBan(ban.Id, banUpdateModel, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal("Test", result.Reason);
        Assert.Equal(banUpdateModel.BanEndDate, result.BanEndDate);
    }
    
    [Fact]
    public async Task UpdateBan_WhenBanDoesNotExist_ShouldThrowException()
    {
        var banUpdateModel = new BanUpdateModel
        (
            DateTime.Now.AddDays(2),
            "Test"
        );

        await Assert.ThrowsAsync<NotFound>(() => _banService.UpdateBan(0, banUpdateModel, CancellationToken.None));
    }
    
    [Fact]
    public async Task DeleteBan_WhenBanExists_ShouldDeleteBan()
    {
        var ban = _serviceMock.RandomDataGenerator.GeneratedBanData.First();
        
        await _banService.DeleteBan(ban.Id, CancellationToken.None);
        
        await Assert.ThrowsAsync<NotFound>(()=>_banService.GetBanById(ban.Id, CancellationToken.None));
    }
    
    [Fact]
    public async Task DeleteBan_WhenBanDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => _banService.DeleteBan(0, CancellationToken.None));
    }

    #endregion
}