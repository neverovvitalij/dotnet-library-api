

using dotnet_library_api.Application.Books.Models;
using dotnet_library_api.Application.Common;
using MediatR;

namespace dotnet_library_api.Application.Books.Queries;
public record GetBooksQuery(int Page, int PageSize) : IRequest<PagedResult<BookReadModel>>;
