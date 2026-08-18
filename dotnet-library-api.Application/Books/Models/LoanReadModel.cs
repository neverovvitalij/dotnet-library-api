

namespace dotnet_library_api.Application.Books.Models;
public record LoanReadModel
(
    int Id,
    string BookTitle,
    string BorrowerName,
    DateTime LoanDate,
    DateTime? ReturnDate
);
