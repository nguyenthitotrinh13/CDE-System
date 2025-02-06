using CDE.Models;

namespace CDE.Repository
{
    public interface IDistributorRepository
    {
        Task<List<Distributor>> GetDistributorsByAreaNameAsync(string areaName);
        Task<bool> DeleteDistributorByIdAsync(int distributorId);
        Task<bool> AddDistributorToArea(AddDistributorRequest request);
    }
}
