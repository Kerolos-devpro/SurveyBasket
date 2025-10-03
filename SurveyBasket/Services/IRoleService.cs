using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includedDisabled = false , CancellationToken cancellationToken = default);
    Task<Result<RoleDetailResponse>> GetAsync(string Id);
    Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request);
    Task<Result> UpdateAsync(string id, RoleRequest request);
    Task<Result> ToggleStatusAsync(string id);
}
