

using dotnet_library_api.Application.Books.Models;
using dotnet_library_api.Application.Common;
using dotnet_library_api.Application.Interfaces;
using MediatR;

namespace dotnet_library_api.Application.Books.Queries;
public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, PagedResult<BookReadModel>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<PagedResult<BookReadModel>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetPagedAsync(request.Page, request.PageSize);
        var booksReadModel = books.Items.Select(b => new BookReadModel(b.Id, b.Title, b.PublishedYear, b.Author.Name, b.Genres.Select(g => g.Name).ToList())).ToList();
        var result = new PagedResult<BookReadModel>(booksReadModel, books.TotalCount);
        return result;
    }
}
