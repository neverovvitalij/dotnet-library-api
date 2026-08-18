using dotnet_library_api.Application.Books.Commands;
using dotnet_library_api.Application.Books.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.DTOs.V1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LoansController : ControllerBase
{
    private readonly ILoanRepository _loanRepository;
    private readonly IMediator _mediator;
    public LoansController(ILoanRepository loanRepository, IMediator mediator)
    {
        _loanRepository = loanRepository;
        _mediator = mediator;
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
        try
        {
        var result = await _mediator.Send(new CreateLoanCommand(createLoanDto.BookId, createLoanDto.BorrowerName));

        if (result.Error == CreateLoanError.BookNotFound)
        {
            return NotFound("Das Buch wurde nicht gefunden");
        }

        if (result.Error == CreateLoanError.AlreadyLoaned)
        {
            return Conflict("Das Buch ist bereits ausgeliehen");
        }

        var loanDto = new LoanDto(result.Loan!.Id, result.Loan.BookTitle, result.Loan.BorrowerName, result.Loan.LoanDate, result.Loan.ReturnDate);
        return CreatedAtAction(nameof(GetLoanById), new {id = result.Loan.Id}, loanDto);

        }
        catch (DbUpdateException)
        {
            return Conflict("Das Buch ist bereits ausgeliehen");
        }

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
