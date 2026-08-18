
using dotnet_library_api.Application.Books.Models;
using MediatR;

namespace dotnet_library_api.Application.Books.Commands;
public record CreateLoanCommand(int BookId, string BorrowerName) :IRequest<CreateLoanResult>;

