using Common.DTOs;
using Common.DTOs.Ban.Request;
using Common.DTOs.Ban.Response;
using Common.Parameters;

namespace Services.Contracts.Contracts;

public interface IBanService
{
    static Dictionary<string, string> BlackList { get; } = new();

    Task<IEnumerable<BanResponseModel>> GetAllBans(CancellationToken cancellationToken, RequestParameters parameters);
    Task<BanResponseModel> GetBanById(uint banId, CancellationToken cancellationToken);
    Task<BanResponseModel> GetBanByUserId(string userId, CancellationToken cancellationToken);
    
    Task<BanResponseModel> CreateBan(BanCreateModel banCreateModel, CancellationToken cancellationToken);
    Task<BanResponseModel> UpdateBan(uint banId, BanUpdateModel banUpdateModel, CancellationToken cancellationToken);
    Task DeleteBan(uint banId, CancellationToken cancellationToken);
    Task DeleteBan(string userId, CancellationToken cancellationToken);
    
    Task<IEnumerable<ActionLogResponse>> GetBanLogs(string userId, uint banId, CancellationToken cancellationToken);
    Task RevokeExpiredBans(CancellationToken cancellationToken);
}