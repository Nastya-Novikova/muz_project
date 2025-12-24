using System.Text.Json;

namespace backend.Services.Interfaces;

public interface IProfileService
{
    Task<JsonDocument?> SearchAsync(JsonDocument searchParams);
    Task<JsonDocument?> GetByIdAsync(Guid id);
    Task<JsonDocument?> GetByUserIdAsync(Guid userId);
    Task<JsonDocument?> CreateAsync(JsonDocument profileJson, Guid userId);
    Task<JsonDocument?> UpdateAsync(Guid id, JsonDocument profileJson, Guid userId);
    Task<JsonDocument?> DeleteAsync(Guid id);
    Task<JsonDocument?> GetFullProfileAsync(Guid userId);
}