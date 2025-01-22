using CDE.DBContexts;
using CDE.Models;
using Microsoft.EntityFrameworkCore;

namespace CDE.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _context.AreaLists.ToListAsync();  // Lấy tất cả các Area từ cơ sở dữ liệu
        }
        public async Task<IEnumerable<Area>> GetAreasAsync(string search, string sortBy, bool ascending)
        {
            var query = _context.AreaLists.AsQueryable();

            // Nếu search là null hoặc chuỗi rỗng, không áp dụng điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    a.AreaName.Contains(search) ||
                    a.DistributorCity.ToString().Contains(search) ||
                    a.AreaCode.ToString().Contains(search));
            }

            // Sắp xếp
            query = sortBy switch
            {
                "AreaCode" => ascending ? query.OrderBy(a => a.AreaCode) : query.OrderByDescending(a => a.AreaCode),
                "AreaName" => ascending ? query.OrderBy(a => a.AreaName) : query.OrderByDescending(a => a.AreaName),
                _ => query.OrderBy(a => a.AreaCode),
            };

            return await query.ToListAsync();
        }

        public async Task<Area> GetAreaByIdAsync(int id)
        {
            return await _context.AreaLists.FindAsync(id);
        }

        public async Task<Area> CreateAreaAsync(Area area)
        {
            _context.AreaLists.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            var area = await _context.AreaLists.FindAsync(id);
            if (area == null)
            {
                return false;
            }

            _context.AreaLists.Remove(area);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
