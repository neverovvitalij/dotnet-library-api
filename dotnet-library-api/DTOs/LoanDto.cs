namespace dotnet_library_api.DTOs;

public record LoanDto
(
    int Id,
    string BookTitle,
    string BorrowerName,
    DateTime LoanDate,
    DateTime? ReturnDate
);
