using dotnet_library_api.DTOs;
using dotnet_library_api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using dotnet_library_api.Application.Interfaces;

namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    public LoansController(ILoanRepository loanRepository, IBookRepository bookRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
    {
        var loans = await _loanRepository.GetAllAsync();
        var loansDtos = loans.Select(l => new LoanDto(l.Id, l.Book.Title, l.BorrowerName, l.LoanDate, l.ReturnDate)).ToList();
        return Ok(loansDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetLoanById(int id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        if (loan == null)
        {
            return NotFound("Loan wurde nicht gefunden");
        }
        var loanDto = new LoanDto(loan.Id, loan.Book.Title, loan.BorrowerName, loan.LoanDate, loan.ReturnDate);
        return Ok(loanDto);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> CreateLoan(CreateLoanDto createLoanDto)
    {
        var book = await _bookRepository.GetByIdAsync(createLoanDto.BookId);
        if (book == null)
        {
            return NotFound("Das Buch wurde nicht gefunden");
        }

        bool isAlreadyLoaned = await _loanRepository.HasActiveLoanAsync(createLoanDto.BookId);
        if (isAlreadyLoaned)
        {
            return Conflict("Das Buch ist bereits ausgeliehen");
        }

        var loan = new Loan
        {
            BookId = createLoanDto.BookId,
            BorrowerName = createLoanDto.BorrowerName,
            LoanDate = DateTime.UtcNow,
            ReturnDate = null
        };
        await _loanRepository.AddAsync(loan);
        await _loanRepository.SaveChangesAsync();

        var loanDto = new LoanDto(loan.Id, book.Title, loan.BorrowerName, loan.LoanDate, loan.ReturnDate);
        return CreatedAtAction(nameof(GetLoanById), new {id = loan.Id}, loanDto);
    }

    [HttpPut("{id}/return")]
    public async Task<ActionResult<LoanDto>> ReturnBook(int id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        if(loan == null)
        {
            return NotFound("Die Ausleihe wurde nicht gefunden");
        }
        if(loan.ReturnDate != null)
        {
            return Conflict("Das buch wurde bereits zurückgegeben");
        }

        loan.ReturnDate = DateTime.UtcNow;
        await _loanRepository.SaveChangesAsync();

        var loanDto = new LoanDto(loan.Id, loan.Book.Title, loan.BorrowerName, loan.LoanDate, loan.ReturnDate);
        return Ok(loanDto);
    }
}
