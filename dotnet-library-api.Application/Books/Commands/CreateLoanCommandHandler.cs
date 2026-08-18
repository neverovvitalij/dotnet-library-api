
using dotnet_library_api.Application.Books.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using MediatR;

namespace dotnet_library_api.Application.Books.Commands;
public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand ,CreateLoanResult>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;

    public CreateLoanCommandHandler(ILoanRepository loanRepository, IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
    }

    public async Task<CreateLoanResult> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if(book == null)
        {
            return new CreateLoanResult(null, CreateLoanError.BookNotFound);
        }
        
        bool isAlreadyLoaned = await _loanRepository.HasActiveLoanAsync(request.BookId);
        if (isAlreadyLoaned)
        {
            return new CreateLoanResult(null, CreateLoanError.AlreadyLoaned);
        }

        var loan = new Loan
        {
            BookId = book.Id,
            BorrowerName = request.BorrowerName,
            LoanDate = DateTime.UtcNow,
            ReturnDate = null
        };

        await _loanRepository.AddAsync(loan);
        await _loanRepository.SaveChangesAsync();
       
        var loanReadModel = new LoanReadModel(loan.Id, book.Title, request.BorrowerName, loan.LoanDate, loan.ReturnDate);
        return new CreateLoanResult(loanReadModel, CreateLoanError.None);
    }
}
