using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface ICollaborationGoalRepository
{
    DbSet<CollaborationGoal> CollaborationGoals { get; }
    Task<List<CollaborationGoal>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
    Task<CollaborationGoal?> GetByIdAsync(int id);
}