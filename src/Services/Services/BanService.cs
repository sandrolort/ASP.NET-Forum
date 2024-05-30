using Common.DTOs;
using Common.DTOs.Ban.Request;
using Common.DTOs.Ban.Response;
using Common.Exceptions;
using Common.Parameters;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Repository.Contracts;
using Services.Contracts.Contracts;

namespace Services.Services;

public class BanService : IBanService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerService _logger;
    private readonly UserManager<User> _userManager;
    
    public BanService(IRepositoryManager repositoryManager, ILoggerService logger, UserManager<User> userManager)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
        _userManager = userManager;
    }
    
    public async Task<IEnumerable<BanResponseModel>> GetAllBans(CancellationToken cancellationToken, RequestParameters parameters)
    {
        var bans = await _repositoryManager.BanRepository.GetBans(parameters, cancellationToken);
        return bans.Adapt<IEnumerable<BanResponseModel>>();
    }

    public async Task<BanResponseModel> GetBanById(uint banId, CancellationToken cancellationToken)
    {
        var ban = await CheckAndReturnBan(banId, false, cancellationToken);
        return ban.Adapt<BanResponseModel>();
    }

    public async Task<BanResponseModel> GetBanByUserId(string userId, CancellationToken cancellationToken)
    {
        var ban = await _repositoryManager.BanRepository.GetBanByUserId(userId, false, cancellationToken)
            ?? throw new NotFound("No such Ban found.");
        return ban.Adapt<BanResponseModel>();
    }

    public async Task<BanResponseModel> CreateBan(BanCreateModel banCreateModel, CancellationToken cancellationToken)
    {
        Console.WriteLine("Creating ban.");

        var user = await _repositoryManager.UserRepository.GetUserById(banCreateModel.UserId!, true, cancellationToken)
                   ?? throw new NotFound("No such User found.");
        
        if (user.IsBanned) throw new Conflict("User is already banned.");
        
        var ban = banCreateModel.Adapt<Ban>();
        if (ban.BanEndDate < DateTime.Now)
            throw new BadRequest("Ban end date cannot be in the past.");
        
        await _userManager.RemoveFromRoleAsync(user, "User");
        Console.WriteLine("removing "+user.LastJwtToken + " from "+user.Id);
        if (user.LastJwtToken != null) 
            IBanService.BlackList.TryAdd(user.LastJwtToken, user.Id);

        user.IsBanned = true;
        _repositoryManager.BanRepository.Create(ban);

        await _repositoryManager.SaveAsync(cancellationToken);
        return ban.Adapt<BanResponseModel>();
    }

    public async Task<BanResponseModel> UpdateBan(uint banId, BanUpdateModel banUpdateModel, CancellationToken cancellationToken)
    {
        var ban = await CheckAndReturnBan(banId, true, cancellationToken);
        
        ban.Reason = banUpdateModel.Reason ?? ban.Reason;
        ban.BanEndDate = banUpdateModel.BanEndDate ?? ban.BanEndDate;
        
        if (ban.BanEndDate < DateTime.Now)
            throw new BadRequest("Ban end date cannot be in the past.");
        
        await _repositoryManager.SaveAsync(cancellationToken);
        
        return ban.Adapt<BanResponseModel>();
    }

    public async Task DeleteBan(uint banId, CancellationToken cancellationToken)
    {
        var ban = await CheckAndReturnBan(banId, false, cancellationToken);
        await UnbanUser(ban, ban.UserId, cancellationToken);
    }

    public async Task DeleteBan(string userId, CancellationToken cancellationToken)
    {
        var ban = await _repositoryManager.BanRepository.GetBanByUserId(userId, false, cancellationToken);
        await UnbanUser(ban, userId, cancellationToken);
    }

    public async Task<IEnumerable<ActionLogResponse>> GetBanLogs(string userId, uint banId, CancellationToken cancellationToken)
    {
        var logs = await _repositoryManager.ActionLogRepository.GetBanLogs(banId, cancellationToken);
        return logs.Adapt<IEnumerable<ActionLogResponse>>();
    }

    public async Task RevokeExpiredBans(CancellationToken cancellationToken)
    {
        var expiredBans = await _repositoryManager.BanRepository.GetExpiredBans(false, cancellationToken);

        var enumerable = expiredBans as Ban[] ?? expiredBans.ToArray();
        _logger.LogInfo($"Found {enumerable.Length} expired bans.");
        
        foreach (var ban in enumerable)
        {
            await UnbanUser(ban, ban.UserId, cancellationToken);
        }
        await _repositoryManager.SaveAsync(cancellationToken);
    }

    private async Task<Ban> CheckAndReturnBan(uint banId, bool trackChanges, CancellationToken cancellationToken) =>
        await _repositoryManager.BanRepository.GetBanById(banId, trackChanges, cancellationToken) 
        ?? throw new NotFound("No such Ban found.");
    
    private async Task UnbanUser(Ban? ban, string userId, CancellationToken cancellationToken)
    {
        if (ban is null) throw new NotFound("No such Ban found.");
        
        _repositoryManager.BanRepository.Delete(ban);
        
        var user = await _userManager.FindByIdAsync(userId);
        
        user.IsBanned = false;
        
        await _userManager.AddToRoleAsync(user, "User");
        
        var blackListedUser = IBanService.BlackList.FirstOrDefault(x => x.Value == user.Id);
        
        if (blackListedUser is { Key: not null } && !IBanService.BlackList.Remove(blackListedUser.Key))
            _logger.LogError($"Failed to remove {user.Id} from the blacklist.");
        
        await _repositoryManager.SaveAsync(cancellationToken);
    }
}