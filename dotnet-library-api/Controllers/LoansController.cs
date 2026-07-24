using dotnet_library_api.Data;
using dotnet_library_api.DTOs;
using dotnet_library_api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LibraryDbContext _libraryDbContext;
    public LoansController(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
    {
        var loans = await _libraryDbContext.Loans.Select(l => new LoanDto(l.Id, l.Book.Title, l.BorrowerName, l.LoanDate, l.ReturnDate)).ToListAsync();
        return Ok(loans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetLoanById(int id)
    {
        var loan = await _libraryDbContext.Loans.Where(l => l.Id == id).Select(l => new LoanDto(l.Id, l.Book.Title, l.BorrowerName, l.LoanDate, l.ReturnDate)).FirstOrDefaultAsync();
        if (loan == null)
        {
            return NotFound("Loan wurde nicht gefunden");
        }
        return Ok(loan);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> CreateLoan(CreateLoanDto createLoanDto)
    {
        var book = await _libraryDbContext.Books.FindAsync(createLoanDto.BookId);
        if (book == null)
        {
            return NotFound("Das Buch wurde nicht gefunden");
        }

        bool isAlreadyLoaned = await _libraryDbContext.Loans.AnyAsync(l => l.BookId == book.Id && l.ReturnDate == null);
        if (isAlreadyLoaned)
        {
            return Conflict("Das Buch ist bereits ausgeliehen");
        }

        var loan = new Loan
        {
            BookId = book.Id,
            BorrowerName = createLoanDto.BorrowerName,
            LoanDate = DateTime.UtcNow,
            ReturnDate = null
        };
        _libraryDbContext.Loans.Add(loan);
        await _libraryDbContext.SaveChangesAsync();

        var loanDto = new LoanDto(loan.Id, book.Title, loan.BorrowerName, loan.LoanDate, loan.ReturnDate);
        return CreatedAtAction(nameof(GetLoanById), new {id = loan.Id}, loanDto);
    }

    [HttpPut("{id}/return")]
    public async Task<ActionResult<LoanDto>> ReturnBook(int id)
    {
        var loan = await _libraryDbContext.Loans.Include(l => l.Book).Where(l => l.Id == id).FirstOrDefaultAsync();
        if(loan == null)
        {
            return NotFound("Die Ausleihe wurde nicht gefunden");
        }
        if(loan.ReturnDate != null)
        {
            return Conflict("Das buch wurde bereits zurückgegeben");
        }

        loan.ReturnDate = DateTime.UtcNow;
        await _libraryDbContext.SaveChangesAsync();

        var loanDto = new LoanDto(loan.Id, loan.Book.Title, loan.BorrowerName, loan.LoanDate, loan.ReturnDate);
        return Ok(loanDto);
    }
}
