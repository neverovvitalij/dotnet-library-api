using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface ILoanRepository
{
    Task<List<Loan>> GetAllAsync();
    Task<Loan?> GetByIdAsync(int id);
    Task AddAsync(Loan loan);
    Task<bool> SaveChangesAsync();
    Task<bool> HasActiveLoanAsync(int bookId);
}
