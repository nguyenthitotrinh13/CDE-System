using CDE.Models;

namespace CDE.Repository
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<IEnumerable<Area>> GetAreasAsync(string search, string sortBy, bool ascending);
        Task<Area> GetAreaByIdAsync(int id);
        Task<Area> CreateAreaAsync(Area area);
        Task<bool> DeleteAreaAsync(int id);
       
    }
}
