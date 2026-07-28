using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Infrastructure.Repositories;
public class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public LoanRepository(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }
    public async Task<List<Loan>> GetAllAsync()
    {
        return await _libraryDbContext.Loans.Include(l => l.Book).ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await _libraryDbContext.Loans.Include(l => l.Book).Where(l => l.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddAsync(Loan loan)
    {
        await _libraryDbContext.Loans.AddAsync(loan);
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await _libraryDbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> HasActiveLoanAsync(int bookId)
    {
        return await _libraryDbContext.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);
    }
}
