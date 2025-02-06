using CDE.Models;
using Microsoft.Graph.Models;

namespace CDE.Repository
{
    public interface IAreaRepository
    {
 
        Task<bool> RemoveUserFromArea(string userId, int areaId);
        Task<IEnumerable<UserWithRoles>> GetUsersAsync(string search);
        Task<bool> AssignUserToArea(string userId, int areaId);
        Task<IEnumerable<UserWithRoles>> GetUsersByAreaIdAsync(string areaName);

        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<IEnumerable<Area>> GetAreasAsync(string search, string sortBy, bool ascending);
        Task<Area> GetAreaByIdAsync(int id);
        Task<Area> CreateAreaAsync(Area area);
        Task<bool> DeleteAreaAsync(int id);
       
    }
}
