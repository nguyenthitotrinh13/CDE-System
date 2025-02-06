using CDE.DBContexts;
using CDE.Models;
using Microsoft.EntityFrameworkCore;

namespace CDE.Repository
{
    public class DistributorRepository : IDistributorRepository
    {
        private readonly ApplicationDbContext _context;

        public DistributorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<List<Distributor>> GetDistributorsByAreaNameAsync(string areaName)
        //{
        //    var area = await _context.AreaLists
        //        .FirstOrDefaultAsync(a => a.AreaName == areaName);

        //    if (area == null) return null;

        //    return await _context.Distributors
        //        .Where(d => d.DistributorId == area.DistributorId)
        //        .ToListAsync();
        //}

        public async Task<List<Distributor>> GetDistributorsByAreaNameAsync(string areaName)
        {
            // Lấy tất cả distributorId từ AreaLists có areaName
            var distributorIds = await _context.AreaLists
                .Where(a => a.AreaName == areaName)
                .Select(a => a.DistributorId) // Lấy DistributorId từ bảng AreaLists
                .ToListAsync();

            if (!distributorIds.Any()) return new List<Distributor>();

            // Lấy tất cả Distributor có Id thuộc danh sách trên
            return await _context.Distributors
                .Where(d => distributorIds.Contains(d.DistributorId))
                .ToListAsync();
        }



        public async Task<bool> DeleteDistributorByIdAsync(int distributorId)
        {
            var distributor = await _context.Distributors
                .FirstOrDefaultAsync(d => d.DistributorId == distributorId);

            if (distributor == null) return false;

            _context.Distributors.Remove(distributor);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddDistributorToArea(AddDistributorRequest request)
        {
            var area = await _context.AreaLists
                .FirstOrDefaultAsync(a => a.AreaName == request.AreaName);
            if (area == null) return false;

            var existingDistributor = await _context.Distributors
                .FirstOrDefaultAsync(d => d.Email == request.Email);

            if (existingDistributor != null)
            {
                var distributorExists = await _context.Distributors
                    .AnyAsync(d => d.DistributorId == existingDistributor.DistributorId);

                if (!distributorExists)
                {
                    return false;
                }

                area.DistributorId = existingDistributor.DistributorId;
            }
            else
            {
                var newDistributor = new Distributor
                {
                    Name = request.Name ?? "Unknown Distributor",
                    Address = request.Address,
                    Email = request.Email,
                    Phone = request.Phone
                };

                _context.Distributors.Add(newDistributor);
                await _context.SaveChangesAsync();

                area.DistributorId = newDistributor.DistributorId;
            }

            _context.AreaLists.Update(area);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
