using CDE.DBContexts;
using CDE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.Security;
using System.Collections;
using System.Linq;

namespace CDE.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<UserWithRoles>> GetUsersAsync(string search)
        {
            var query = _context.Users.AsQueryable();

            // Kiểm tra nếu có search input, tìm theo tên người dùng hoặc email
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.UserName.Contains(search) || u.Email.Contains(search));
            }

            
            var usersWithRoles = await (from user in query
                                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                        join role in _context.Roles on userRole.RoleId equals role.Id
                                        select new UserWithRoles
                                        {
                                            UserName = user.UserName,
                                            Email = user.Email,
                                            Roles = string.Join(", ", new[] { role.Name }) // Chỉ lấy tên role
                                        }).ToListAsync();

            return usersWithRoles;
        }


        public async Task<bool> AssignUserToArea(string userId, int areaId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var area = await _context.AreaLists.FirstOrDefaultAsync(a => a.AreaCode == areaId);

            if (user == null || area == null)
            {
                return false;
            }

            area.UserId = userId; 

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserWithRoles>> GetUsersByAreaIdAsync(string areaName)
        {
            var userIdsInArea = await _context.AreaLists
                .Where(a => a.AreaName == areaName)  
                .Select(a => a.UserId)  
                .ToListAsync();

            var usersWithRoles = await (from user in _context.Users
                                        where userIdsInArea.Contains(user.Id)  
                                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                        join role in _context.Roles on userRole.RoleId equals role.Id
                                        select new UserWithRoles
                                        {
                                            UserName = user.UserName,
                                            Email = user.Email,
                                            Roles = string.Join(", ", _context.Roles
                                                                      .Where(r => userRole.RoleId == r.Id)
                                                                      .Select(r => r.Name))
                                        }).ToListAsync();

            return usersWithRoles;
        }

        public async Task<bool> RemoveUserFromArea(string userId, int areaId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var areaListEntry = await _context.AreaLists
                .FirstOrDefaultAsync(al => al.UserId == userId && al.AreaCode == areaId); 

            if (areaListEntry != null)
            {
                _context.AreaLists.Remove(areaListEntry);
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _context.AreaLists.ToListAsync();  
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
