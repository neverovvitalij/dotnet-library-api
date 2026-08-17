

using dotnet_library_api.Application.Books.Models;
using MediatR;

namespace dotnet_library_api.Application.Books.Commands;
public record CreateBookCommand(string Title, int PublishedYear, int AuthorId, List<int> GenreIds) : IRequest<BookReadModel?>;
