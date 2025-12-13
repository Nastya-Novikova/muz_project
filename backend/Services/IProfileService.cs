using System.Text.Json;

namespace backend.Services
{
    public interface IProfileService
    {
        Task<string> SearchAsync(string? city = null, string? genre = null);
        Task<string?> GetByIdAsync(Guid id);
        Task<string> CreateAsync(JsonDocument json);
        Task<bool> DeleteAsync(Guid id);
    }
}
