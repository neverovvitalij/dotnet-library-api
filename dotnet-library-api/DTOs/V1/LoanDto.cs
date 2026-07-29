namespace dotnet_library_api.DTOs.V1;

public record LoanDto
(
    int Id,
    string BookTitle,
    string BorrowerName,
    DateTime LoanDate,
    DateTime? ReturnDate
);
